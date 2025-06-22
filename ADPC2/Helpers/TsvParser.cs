using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADPC2.Models;
using System.IO.Compression;

namespace ADPC2.Helpers
{
    public class TsvParser
    {

        private static readonly HashSet<string> relevantGenes = new HashSet<string>
            {
                "C6orf150", "CCL5", "CXCL10", "TMEM173", "CXCL9", "CXCL11",
                "NFKB1", "IKBKE", "IRF3", "TREX1", "ATM", "IL6", "IL8"
            };

        public static List<GeneExpression> ParseGeneExpressions(Stream gzipStream, string cohortName)
        {
            var result = new Dictionary<string, GeneExpression>();

            try
            {
                using var decompressed = new GZipStream(gzipStream, CompressionMode.Decompress);
                using var reader = new StreamReader(decompressed);
                using var parser = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = "\t",
                    BadDataFound = null
                });

                parser.Read();
                parser.ReadHeader();

                var ids = parser.HeaderRecord.Skip(1).Select(x => x?.Trim().ToUpper()).ToList();

                while (parser.Read())
                {
                    var gene = parser.GetField<string>(0);

                    if (relevantGenes.Contains(gene))
                    {
                        for (int i = 0; i < ids.Count; i++)
                        {
                            var id = ids[i];
                            if (string.IsNullOrEmpty(id)) continue;

                            if (!result.ContainsKey(id))
                            {
                                result[id] = new GeneExpression
                                {
                                    PatientId = id,
                                    CancerCohort = cohortName,
                                    GeneValues = new Dictionary<string, double>()
                                };
                            }

                            if (double.TryParse(parser.GetField(i + 1), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                            {
                                result[id].GeneValues[gene] = val;
                            }
                        }
                    }
                }

                Console.WriteLine($"Parsed {result.Count} patients with target gene data");
                return result.Values.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing gene expression data: {ex.Message}");
                return new List<GeneExpression>();
            }
        }



        public static Dictionary<string, ClinicalSurvival> ParseClinicalData(string filePath)
        {
            var clinicalData = new Dictionary<string, ClinicalSurvival>();

            try
            {
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = "\t",
                    BadDataFound = null
                });

                csv.Read();
                csv.ReadHeader();

               
                int colBarcode = -1;
                if (csv.HeaderRecord.Contains("sample"))
                    colBarcode = csv.GetFieldIndex("sample");
                else if (csv.HeaderRecord.Contains("bcr_patient_barcode"))
                    colBarcode = csv.GetFieldIndex("bcr_patient_barcode");
                else
                    colBarcode = 0; 

                int colDSS = csv.GetFieldIndex("DSS", isTryGet: true);
                int colOS = csv.GetFieldIndex("OS", isTryGet: true);
                int colStage = csv.GetFieldIndex("clinical_stage", isTryGet: true);

                while (csv.Read())
                {
                    var patientId = csv.GetField(colBarcode)?.Trim().ToUpper();
                    if (string.IsNullOrEmpty(patientId)) continue;

                    int? dss = colDSS >= 0 ? ParseNullableInt(csv.GetField(colDSS)) : null;
                    int? os = colOS >= 0 ? ParseNullableInt(csv.GetField(colOS)) : null;
                    string stage = colStage >= 0 ? csv.GetField(colStage) : null;

                    clinicalData[patientId] = new ClinicalSurvival
                    {
                        PatientId = patientId,
                        DiseaseSpecificSurvival = dss,
                        OverallSurvival = os,
                        ClinicalStage = stage
                    };
                }

                Console.WriteLine($"Parsed {clinicalData.Count} clinical records");
                return clinicalData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing data: {ex.Message}");
                return new Dictionary<string, ClinicalSurvival>();
            }
        }

        private static int? ParseNullableInt(string value)
        {
            if (int.TryParse(value, out var result))
                return result;
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                return (int)d;
            return null;
        }

    }
}
