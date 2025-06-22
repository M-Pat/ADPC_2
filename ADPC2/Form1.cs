using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADPC2.Models;
using ADPC2.Services;


namespace ADPC2
{
    public partial class Form1 : Form
    {
        private XenaDataService _xenaService;
        private ClinicalParser _clinicalService;
        private MongoService _mongoService;

        public Form1()
        {
            InitializeComponent();
            _xenaService = new XenaDataService();
            _clinicalService = new ClinicalParser();
            _mongoService = new MongoService();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                button1.Enabled = false;
                button1.Text = "Scraping Genes...";
                progressLabel.Text = "🔄 Downloading gene expression files...";
                progressLabel.Refresh();

                await _xenaService.ScrapeAndDownloadFilesAsync();

                var insertedPatients = await _mongoService.GetGeneExpressionsAsync();
                labelPatientCount.Text = $"Patients inserted: {insertedPatients.Count}";

                progressLabel.Text = "📤 Uploading to MongoDB...";
                progressLabel.Refresh();

                await _xenaService.ProcessFilesFromMinIO();

                progressLabel.Text = "✅ Gene data saved to MongoDB.";
                MessageBox.Show("Gene expression data scraped, uploaded, and stored in MongoDB.");
            }
            catch (Exception ex)
            {
                progressLabel.Text = "❌ Error during gene scraping.";
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                button1.Enabled = true;
                button1.Text = "Scrape Gene Data";
            }

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                button2.Enabled = false;
                button2.Text = "Scraping Clinical...";
                progressLabel.Text = "🔄 Downloading clinical files...";
                progressLabel.Refresh();

                await _clinicalService.ScrapeAndDownloadClinicalFilesAsync();

                progressLabel.Text = "📤 Uploaded clinical data to MinIO.";
                MessageBox.Show("Clinical data scraped and uploaded to MinIO.");
            }
            catch (Exception ex)
            {
                progressLabel.Text = "❌ Error during clinical scraping.";
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                button2.Enabled = true;
                button2.Text = "Scrape Clinical Data";
            }

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                button3.Enabled = false;
                button3.Text = "Merging...";
                progressLabel.Text = "🔄 Merging clinical with gene expression data...";
                progressLabel.Refresh();

                await _clinicalService.MergeClinicalWithGeneExpressionAsync(_mongoService);

                progressLabel.Text = "✅ Merged clinical data with gene expression in MongoDB.";
                MessageBox.Show("Clinical and gene expression data merged into MongoDB.");
                await LoadPatientsIntoDropdown();
            }
            catch (Exception ex)
            {
                progressLabel.Text = "❌ Error during data merge.";
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                button3.Enabled = true;
                button3.Text = "Merge Clinical + Gene Data";
            }
        }

        private async Task LoadPatientsIntoDropdown()
        {
            var patients = await _mongoService.GetGeneExpressionsAsync();
            var ids = patients.Select(p => p.PatientId).Distinct().OrderBy(id => id).ToArray();

            comboBoxPatients.Items.Clear();
            comboBoxPatients.Items.AddRange(ids);

            if (comboBoxPatients.Items.Count > 0)
                comboBoxPatients.SelectedIndex = 0;
        }

        private void ShowGeneChart(GeneExpression patient)
        {
            var targetGenes = new[]
            {
        "C6orf150", "CCL5", "CXCL10", "TMEM173", "CXCL9", "CXCL11",
        "NFKB1", "IKBKE", "IRF3", "TREX1", "ATM", "IL6", "IL8"
    };

            chartGenes.Series.Clear();
            chartGenes.ChartAreas.Clear();

            var area = chartGenes.ChartAreas.Add("MainArea");
            var series = chartGenes.Series.Add("GeneExpression");
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;

            foreach (var gene in targetGenes)
            {
                if (patient.GeneValues.TryGetValue(gene, out var value))
                {
                    var pointIndex = series.Points.AddXY(gene, value);
                    series.Points[pointIndex].ToolTip = $"{gene}: {value:F2}";
                }
            }

            chartGenes.Visible = true;
        }


        private async void buttonViewPatient_Click(object sender, EventArgs e)
        {
            string id = textBoxPatientId.Text.Trim().ToUpper();

            if (string.IsNullOrEmpty(id) && comboBoxPatients.SelectedItem != null)
            {
                id = comboBoxPatients.SelectedItem.ToString().Trim().ToUpper();
            }

            if (string.IsNullOrEmpty(id)) return;

            var patient = await _mongoService.GetExpressionByPatientIdAsync(id);
            if (patient != null)
            {
                labelPatientInfo.Text = $"Patient: {patient.PatientId}, Cohort: {patient.CancerCohort}, " +
                                        $"Genes: {patient.GeneValues.Count}, DSS: {patient.Clinical?.DiseaseSpecificSurvival}";

                ShowGeneChart(patient);
            }
            else
            {
                labelPatientInfo.Text = "Patient not found.";
                chartGenes.Visible = false;
            }
        }

        private async void buttonExportCsv_Click(object sender, EventArgs e)
        {
            var allPatients = await _mongoService.GetGeneExpressionsAsync();
            if (allPatients.Count == 0)
            {
                MessageBox.Show("No patient data available.");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                FileName = "GeneDataExport.csv"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                using var writer = new StreamWriter(saveDialog.FileName);
                using var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);

                csv.WriteField("PatientId");
                csv.WriteField("CancerCohort");
                foreach (var gene in allPatients.SelectMany(p => p.GeneValues.Keys).Distinct())
                    csv.WriteField(gene);
                csv.NextRecord();

                foreach (var patient in allPatients)
                {
                    csv.WriteField(patient.PatientId);
                    csv.WriteField(patient.CancerCohort);
                    foreach (var gene in allPatients.SelectMany(p => p.GeneValues.Keys).Distinct())
                        csv.WriteField(patient.GeneValues.ContainsKey(gene) ? patient.GeneValues[gene] : 0);
                    csv.NextRecord();
                }

                MessageBox.Show("Exported to CSV.");
            }
        }
    }
}
