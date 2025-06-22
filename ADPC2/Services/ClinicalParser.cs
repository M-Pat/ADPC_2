using Minio.DataModel.Args;
using Minio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.Net.Http;
using ADPC2.Models.Constraints;
using ADPC2.Models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Minio.ApiEndpoints;
using MongoDB.Driver;
using System.Reactive.Linq;
using ADPC2.Helpers;

namespace ADPC2.Services
{
    public  class ClinicalParser
    {
        private readonly string mainUrl = "https://xenabrowser.net/datapages/?hub=https://tcga.xenahubs.net:443";
        private readonly string downloadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Clinical");
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName = Constraints.ClinicalBucketName;

        public ClinicalParser()
        {
            _minioClient = new MinioClient()
                .WithEndpoint(Constraints.ENDPOINT)
                .WithCredentials(Constraints.ACCESS_KEY, Constraints.SECRET_KEY)
                .WithSSL(false)
                .Build();

            Directory.CreateDirectory(downloadDirectory);
        }
        public async Task ScrapeAndDownloadClinicalFilesAsync()
        {
            await ClearBucketAsync();

            using (var driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(mainUrl);
                await Task.Delay(5000);

                var links = driver.FindElements(By.XPath("//a[contains(@href, 'cohort=TCGA')]"))
                                  .Select(l => l.GetAttribute("href"))
                                  .ToList();

                foreach (var url in links)
                {
                    driver.Navigate().GoToUrl(url);
                    await Task.Delay(3000);

                    var survivalData = driver.FindElements(By.XPath("//a[contains(text(), 'Curated survival data')]"));
                    if (survivalData.Count == 0)
                    {
                        Console.WriteLine($"No curated survival data for cohort: {url}");
                        continue;
                    }

                    survivalData[0].Click();
                    await Task.Delay(3000);

                    var txtLinks = driver.FindElements(By.XPath("//a[contains(@href, '.txt')]"));
                    if (txtLinks.Count == 0)
                    {
                        Console.WriteLine("No .txt download link found.");
                        driver.Navigate().Back();
                        await Task.Delay(2000);
                        continue;
                    }

                    var href = txtLinks[0].GetAttribute("href");
                    var name = Path.GetFileName(href);
                    var path = Path.Combine(downloadDirectory, name);

                    using (var http = new HttpClient())
                    {
                        await DownloadFile(http, href, path);
                        await UploadFileToMinIO(path);
                    }

                    Console.WriteLine($"Downloaded and uploaded: {name}");

                    driver.Navigate().Back();
                    await Task.Delay(2000);
                }
            }
        }

        public async Task MergeClinicalWithGeneExpressionAsync(MongoService mongoService)
        {
            
            var clinicalFileNames = await ListClinicalFilesInMinIO();

          
            var allClinicalDicts = new List<Dictionary<string, ClinicalSurvival>>();
            foreach (var fileName in clinicalFileNames)
            {
                var localPath = await DownloadClinicalFileFromMinIO(fileName);
                var dict = TsvParser.ParseClinicalData(localPath);
                allClinicalDicts.Add(dict);
            }
            var mergedClinicalDict = new Dictionary<string, ClinicalSurvival>();
            foreach (var dict in allClinicalDicts)
            {
                foreach (var kvp in dict)
                {
                    
                    mergedClinicalDict[kvp.Key] = kvp.Value;
                }
            }
            
            var allGeneExpr = await mongoService.GetGeneExpressionsAsync();

            foreach (var expr in allGeneExpr)
            {
                var barcode = expr.PatientId.Trim().ToUpper();
                if (mergedClinicalDict.TryGetValue(barcode, out var clinical))
                    expr.Clinical = clinical;
            }
            await mongoService.UpdateClinicalDataAsync(allGeneExpr);

            Console.WriteLine("Clinical data successfully merged and updated in MongoDB.");
        }

        [Obsolete]
        private async Task ClearBucketAsync()
        {
            var fileUrls = new List<string>();
            await _minioClient.ListObjectsAsync(new ListObjectsArgs()
                .WithBucket(_bucketName)
                .WithRecursive(true))
                .ForEachAsync(item => fileUrls.Add(item.Key));

            foreach (var fileUrl in fileUrls)
            {
                await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(fileUrl));
            }
        }

        private async Task DownloadFile(HttpClient client, string fileUrl, string filePath)
        {
            client.Timeout = TimeSpan.FromMinutes(15);
            using (var response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }
        }

        private async Task UploadFileToMinIO(string filePath)
        {
            string objectName = Path.GetFileName(filePath);
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucketName) 
                .WithObject(objectName)
                .WithFileName(filePath)
                .WithObjectSize(new FileInfo(filePath).Length)
                .WithContentType("text/tab-separated-values"));
        }

        [Obsolete]
        private async Task<List<string>> ListClinicalFilesInMinIO()
        {
            var fileNames = new List<string>();
            await _minioClient.ListObjectsAsync(new ListObjectsArgs()
                .WithBucket(_bucketName)
                .WithRecursive(true))
                .ForEachAsync(item => fileNames.Add(item.Key));
            return fileNames;
        }

        private async Task<string> DownloadClinicalFileFromMinIO(string fileName)
        {
            var localPath = Path.Combine(downloadDirectory, fileName);
            using var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write);
            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithCallbackStream(stream => stream.CopyTo(fs)));
            return localPath;
        }


    }
}
