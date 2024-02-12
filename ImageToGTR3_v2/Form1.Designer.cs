namespace ImageToZeppOS
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button_PngToTga = new System.Windows.Forms.Button();
            this.button_TgaToPng = new System.Windows.Forms.Button();
            this.label_version = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.radioButton_type2 = new System.Windows.Forms.RadioButton();
            this.radioButton_type1 = new System.Windows.Forms.RadioButton();
            this.checkBox_newAlgorithm = new System.Windows.Forms.CheckBox();
            this.radioButton_type3 = new System.Windows.Forms.RadioButton();
            this.checkBox_oldAlgorithm = new System.Windows.Forms.CheckBox();
            this.numericUpDown_colorCount = new System.Windows.Forms.NumericUpDown();
            this.button_Batch_TgaToPng = new System.Windows.Forms.Button();
            this.button_Batch_PngToTga = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_colorCount)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // button_PngToTga
            // 
            resources.ApplyResources(this.button_PngToTga, "button_PngToTga");
            this.button_PngToTga.Name = "button_PngToTga";
            this.button_PngToTga.UseVisualStyleBackColor = true;
            this.button_PngToTga.Click += new System.EventHandler(this.button_PngToTga_Click);
            // 
            // button_TgaToPng
            // 
            resources.ApplyResources(this.button_TgaToPng, "button_TgaToPng");
            this.button_TgaToPng.Name = "button_TgaToPng";
            this.button_TgaToPng.UseVisualStyleBackColor = true;
            this.button_TgaToPng.Click += new System.EventHandler(this.button_TgaToPng_Click);
            // 
            // label_version
            // 
            resources.ApplyResources(this.label_version, "label_version");
            this.label_version.Name = "label_version";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 32000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // radioButton_type2
            // 
            resources.ApplyResources(this.radioButton_type2, "radioButton_type2");
            this.radioButton_type2.Name = "radioButton_type2";
            this.toolTip1.SetToolTip(this.radioButton_type2, resources.GetString("radioButton_type2.ToolTip"));
            this.radioButton_type2.UseVisualStyleBackColor = true;
            // 
            // radioButton_type1
            // 
            resources.ApplyResources(this.radioButton_type1, "radioButton_type1");
            this.radioButton_type1.Checked = true;
            this.radioButton_type1.Name = "radioButton_type1";
            this.radioButton_type1.TabStop = true;
            this.toolTip1.SetToolTip(this.radioButton_type1, resources.GetString("radioButton_type1.ToolTip"));
            this.radioButton_type1.UseVisualStyleBackColor = true;
            // 
            // checkBox_newAlgorithm
            // 
            resources.ApplyResources(this.checkBox_newAlgorithm, "checkBox_newAlgorithm");
            this.checkBox_newAlgorithm.Name = "checkBox_newAlgorithm";
            this.toolTip1.SetToolTip(this.checkBox_newAlgorithm, resources.GetString("checkBox_newAlgorithm.ToolTip"));
            this.checkBox_newAlgorithm.UseVisualStyleBackColor = true;
            this.checkBox_newAlgorithm.CheckedChanged += new System.EventHandler(this.checkBox_Algorithm_CheckedChanged);
            // 
            // radioButton_type3
            // 
            resources.ApplyResources(this.radioButton_type3, "radioButton_type3");
            this.radioButton_type3.Name = "radioButton_type3";
            this.toolTip1.SetToolTip(this.radioButton_type3, resources.GetString("radioButton_type3.ToolTip"));
            this.radioButton_type3.UseVisualStyleBackColor = true;
            // 
            // checkBox_oldAlgorithm
            // 
            resources.ApplyResources(this.checkBox_oldAlgorithm, "checkBox_oldAlgorithm");
            this.checkBox_oldAlgorithm.Name = "checkBox_oldAlgorithm";
            this.toolTip1.SetToolTip(this.checkBox_oldAlgorithm, resources.GetString("checkBox_oldAlgorithm.ToolTip"));
            this.checkBox_oldAlgorithm.UseVisualStyleBackColor = true;
            this.checkBox_oldAlgorithm.CheckedChanged += new System.EventHandler(this.checkBox_Algorithm_CheckedChanged);
            // 
            // numericUpDown_colorCount
            // 
            resources.ApplyResources(this.numericUpDown_colorCount, "numericUpDown_colorCount");
            this.numericUpDown_colorCount.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.numericUpDown_colorCount.Name = "numericUpDown_colorCount";
            this.numericUpDown_colorCount.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            // 
            // button_Batch_TgaToPng
            // 
            resources.ApplyResources(this.button_Batch_TgaToPng, "button_Batch_TgaToPng");
            this.button_Batch_TgaToPng.Name = "button_Batch_TgaToPng";
            this.button_Batch_TgaToPng.UseVisualStyleBackColor = true;
            this.button_Batch_TgaToPng.Click += new System.EventHandler(this.button_Batch_TgaToPng_Click);
            // 
            // button_Batch_PngToTga
            // 
            resources.ApplyResources(this.button_Batch_PngToTga, "button_Batch_PngToTga");
            this.button_Batch_PngToTga.Name = "button_Batch_PngToTga";
            this.button_Batch_PngToTga.UseVisualStyleBackColor = true;
            this.button_Batch_PngToTga.Click += new System.EventHandler(this.button_Batch_PngToTga_Click);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_Batch_PngToTga);
            this.Controls.Add(this.button_Batch_TgaToPng);
            this.Controls.Add(this.checkBox_oldAlgorithm);
            this.Controls.Add(this.radioButton_type3);
            this.Controls.Add(this.numericUpDown_colorCount);
            this.Controls.Add(this.checkBox_newAlgorithm);
            this.Controls.Add(this.radioButton_type2);
            this.Controls.Add(this.radioButton_type1);
            this.Controls.Add(this.label_version);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button_PngToTga);
            this.Controls.Add(this.button_TgaToPng);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_colorCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button_PngToTga;
        private System.Windows.Forms.Button button_TgaToPng;
        private System.Windows.Forms.Label label_version;
        private System.Windows.Forms.RadioButton radioButton_type1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.RadioButton radioButton_type2;
        private System.Windows.Forms.CheckBox checkBox_newAlgorithm;
        private System.Windows.Forms.NumericUpDown numericUpDown_colorCount;
        private System.Windows.Forms.RadioButton radioButton_type3;
        private System.Windows.Forms.CheckBox checkBox_oldAlgorithm;
        private System.Windows.Forms.Button button_Batch_TgaToPng;
        private System.Windows.Forms.Button button_Batch_PngToTga;
    }
}

