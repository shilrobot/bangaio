namespace BangaiO
{
    partial class RecordForm
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
            this.deviceCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dbLabel = new System.Windows.Forms.Label();
            this.recordBtn = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.histogram1 = new BangaiO.Histogram();
            this.signalDetector1 = new BangaiO.SignalDetector();
            this.eyePlot1 = new BangaiO.EyePlot();
            this.phasePlot2 = new BangaiO.PhasePlot();
            this.vuMeter1 = new BangaiO.VUMeter();
            this.histogram2 = new BangaiO.Histogram();
            this.SuspendLayout();
            // 
            // deviceCombo
            // 
            this.deviceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceCombo.FormattingEnabled = true;
            this.deviceCombo.Location = new System.Drawing.Point(102, 12);
            this.deviceCombo.Name = "deviceCombo";
            this.deviceCombo.Size = new System.Drawing.Size(330, 21);
            this.deviceCombo.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Capture Device:";
            // 
            // dbLabel
            // 
            this.dbLabel.AutoSize = true;
            this.dbLabel.Location = new System.Drawing.Point(169, 63);
            this.dbLabel.Name = "dbLabel";
            this.dbLabel.Size = new System.Drawing.Size(26, 13);
            this.dbLabel.TabIndex = 4;
            this.dbLabel.Text = "(dB)";
            // 
            // recordBtn
            // 
            this.recordBtn.Appearance = System.Windows.Forms.Appearance.Button;
            this.recordBtn.Image = global::BangaiO.Properties.Resources.record;
            this.recordBtn.Location = new System.Drawing.Point(15, 39);
            this.recordBtn.Name = "recordBtn";
            this.recordBtn.Size = new System.Drawing.Size(67, 63);
            this.recordBtn.TabIndex = 7;
            this.recordBtn.Text = "Record";
            this.recordBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.recordBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.recordBtn.UseVisualStyleBackColor = true;
            this.recordBtn.CheckedChanged += new System.EventHandler(this.recordBtn_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(227, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Carrier Detected:";
            // 
            // histogram1
            // 
            this.histogram1.Location = new System.Drawing.Point(259, 282);
            this.histogram1.Name = "histogram1";
            this.histogram1.Size = new System.Drawing.Size(223, 165);
            this.histogram1.TabIndex = 10;
            this.histogram1.Text = "histogram1";
            // 
            // signalDetector1
            // 
            this.signalDetector1.Location = new System.Drawing.Point(318, 56);
            this.signalDetector1.Name = "signalDetector1";
            this.signalDetector1.Size = new System.Drawing.Size(34, 27);
            this.signalDetector1.TabIndex = 8;
            this.signalDetector1.Text = "signalDetector1";
            this.signalDetector1.Click += new System.EventHandler(this.signalDetector1_Click);
            // 
            // eyePlot1
            // 
            this.eyePlot1.Location = new System.Drawing.Point(15, 282);
            this.eyePlot1.Name = "eyePlot1";
            this.eyePlot1.Size = new System.Drawing.Size(238, 165);
            this.eyePlot1.TabIndex = 6;
            this.eyePlot1.Text = "eyePlot1";
            // 
            // phasePlot2
            // 
            this.phasePlot2.Location = new System.Drawing.Point(15, 108);
            this.phasePlot2.Name = "phasePlot2";
            this.phasePlot2.Size = new System.Drawing.Size(695, 168);
            this.phasePlot2.TabIndex = 5;
            this.phasePlot2.Text = "phasePlot2";
            // 
            // vuMeter1
            // 
            this.vuMeter1.Color = System.Drawing.Color.Green;
            this.vuMeter1.Location = new System.Drawing.Point(88, 63);
            this.vuMeter1.Name = "vuMeter1";
            this.vuMeter1.Percent = 0F;
            this.vuMeter1.Size = new System.Drawing.Size(75, 13);
            this.vuMeter1.TabIndex = 3;
            this.vuMeter1.Text = "vuMeter1";
            // 
            // histogram2
            // 
            this.histogram2.Location = new System.Drawing.Point(488, 282);
            this.histogram2.Name = "histogram2";
            this.histogram2.Size = new System.Drawing.Size(223, 165);
            this.histogram2.TabIndex = 11;
            this.histogram2.Text = "histogram2";
            // 
            // RecordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 552);
            this.Controls.Add(this.histogram2);
            this.Controls.Add(this.histogram1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.signalDetector1);
            this.Controls.Add(this.recordBtn);
            this.Controls.Add(this.eyePlot1);
            this.Controls.Add(this.phasePlot2);
            this.Controls.Add(this.dbLabel);
            this.Controls.Add(this.vuMeter1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.deviceCombo);
            this.DoubleBuffered = true;
            this.Name = "RecordForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox deviceCombo;
        private System.Windows.Forms.Label label1;
        private VUMeter vuMeter1;
        private System.Windows.Forms.Label dbLabel;
        private PhasePlot phasePlot2;
        private EyePlot eyePlot1;
        private System.Windows.Forms.CheckBox recordBtn;
        private SignalDetector signalDetector1;
        private System.Windows.Forms.Label label2;
        private Histogram histogram1;
        private Histogram histogram2;
    }
}

