namespace ADPC2
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.progressLabel = new System.Windows.Forms.Label();
            this.textBoxPatientId = new System.Windows.Forms.TextBox();
            this.labelPatientCount = new System.Windows.Forms.Label();
            this.buttonViewPatient = new System.Windows.Forms.Button();
            this.labelPatientInfo = new System.Windows.Forms.Label();
            this.buttonExportCsv = new System.Windows.Forms.Button();
            this.comboBoxPatients = new System.Windows.Forms.ComboBox();
            this.chartGenes = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chartGenes)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(107, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Scrape gene data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(125, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(107, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Scrape clin. data";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(238, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(107, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Merge data";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(12, 38);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(51, 13);
            this.progressLabel.TabIndex = 3;
            this.progressLabel.Text = "Progress:";
            // 
            // textBoxPatientId
            // 
            this.textBoxPatientId.Location = new System.Drawing.Point(440, 18);
            this.textBoxPatientId.Name = "textBoxPatientId";
            this.textBoxPatientId.Size = new System.Drawing.Size(85, 20);
            this.textBoxPatientId.TabIndex = 4;
            // 
            // labelPatientCount
            // 
            this.labelPatientCount.AutoSize = true;
            this.labelPatientCount.Location = new System.Drawing.Point(12, 60);
            this.labelPatientCount.Name = "labelPatientCount";
            this.labelPatientCount.Size = new System.Drawing.Size(97, 13);
            this.labelPatientCount.TabIndex = 6;
            this.labelPatientCount.Text = "Patients inserted: 0";
            // 
            // buttonViewPatient
            // 
            this.buttonViewPatient.Location = new System.Drawing.Point(658, 15);
            this.buttonViewPatient.Name = "buttonViewPatient";
            this.buttonViewPatient.Size = new System.Drawing.Size(75, 23);
            this.buttonViewPatient.TabIndex = 7;
            this.buttonViewPatient.Text = "View patient";
            this.buttonViewPatient.UseVisualStyleBackColor = true;
            this.buttonViewPatient.Click += new System.EventHandler(this.buttonViewPatient_Click);
            // 
            // labelPatientInfo
            // 
            this.labelPatientInfo.AutoSize = true;
            this.labelPatientInfo.Location = new System.Drawing.Point(380, 50);
            this.labelPatientInfo.Name = "labelPatientInfo";
            this.labelPatientInfo.Size = new System.Drawing.Size(0, 13);
            this.labelPatientInfo.TabIndex = 8;
            // 
            // buttonExportCsv
            // 
            this.buttonExportCsv.Location = new System.Drawing.Point(705, 415);
            this.buttonExportCsv.Name = "buttonExportCsv";
            this.buttonExportCsv.Size = new System.Drawing.Size(75, 23);
            this.buttonExportCsv.TabIndex = 9;
            this.buttonExportCsv.Text = "Export";
            this.buttonExportCsv.UseVisualStyleBackColor = true;
            this.buttonExportCsv.Click += new System.EventHandler(this.buttonExportCsv_Click);
            // 
            // comboBoxPatients
            // 
            this.comboBoxPatients.FormattingEnabled = true;
            this.comboBoxPatients.Location = new System.Drawing.Point(531, 17);
            this.comboBoxPatients.Name = "comboBoxPatients";
            this.comboBoxPatients.Size = new System.Drawing.Size(121, 21);
            this.comboBoxPatients.TabIndex = 10;
            // 
            // chartGenes
            // 
            chartArea3.Name = "ChartArea1";
            this.chartGenes.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chartGenes.Legends.Add(legend3);
            this.chartGenes.Location = new System.Drawing.Point(401, 87);
            this.chartGenes.Name = "chartGenes";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chartGenes.Series.Add(series3);
            this.chartGenes.Size = new System.Drawing.Size(332, 300);
            this.chartGenes.TabIndex = 11;
            this.chartGenes.Text = "chart1";
            this.chartGenes.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.chartGenes);
            this.Controls.Add(this.comboBoxPatients);
            this.Controls.Add(this.buttonExportCsv);
            this.Controls.Add(this.labelPatientInfo);
            this.Controls.Add(this.buttonViewPatient);
            this.Controls.Add(this.labelPatientCount);
            this.Controls.Add(this.textBoxPatientId);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chartGenes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.TextBox textBoxPatientId;
        private System.Windows.Forms.Label labelPatientCount;
        private System.Windows.Forms.Button buttonViewPatient;
        private System.Windows.Forms.Label labelPatientInfo;
        private System.Windows.Forms.Button buttonExportCsv;
        private System.Windows.Forms.ComboBox comboBoxPatients;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartGenes;
    }
}

