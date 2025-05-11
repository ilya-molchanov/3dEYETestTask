namespace BackendTestTask.TestFileGenerator
{
    partial class TestFileGeneratorForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnGenerate = new Button();
            lblGigabytesCount = new Label();
            tbGigabytesCount = new TextBox();
            progressBar1 = new ProgressBar();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(134, 141);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(115, 23);
            btnGenerate.TabIndex = 3;
            btnGenerate.Text = "Generate Test File";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // lblGigabytesCount
            // 
            lblGigabytesCount.AutoSize = true;
            lblGigabytesCount.Location = new Point(39, 30);
            lblGigabytesCount.Name = "lblGigabytesCount";
            lblGigabytesCount.Size = new Size(210, 15);
            lblGigabytesCount.TabIndex = 1;
            lblGigabytesCount.Text = "Number of gigabytes (whole number):";
            lblGigabytesCount.Click += lblIterationsCount_Click;
            // 
            // tbGigabytesCount
            // 
            tbGigabytesCount.Location = new Point(255, 30);
            tbGigabytesCount.Name = "tbGigabytesCount";
            tbGigabytesCount.Size = new Size(100, 23);
            tbGigabytesCount.TabIndex = 2;
            tbGigabytesCount.Text = "1";
            tbGigabytesCount.KeyPress += tbIterationsCount_KeyPress;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(39, 100);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(316, 23);
            progressBar1.TabIndex = 4;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(159, 70);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 15);
            lblStatus.TabIndex = 5;
            // 
            // TestFileGeneratorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(399, 178);
            Controls.Add(lblStatus);
            Controls.Add(progressBar1);
            Controls.Add(tbGigabytesCount);
            Controls.Add(lblGigabytesCount);
            Controls.Add(btnGenerate);
            Name = "TestFileGeneratorForm";
            Text = "Test File Generator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnGenerate;
        private Label lblGigabytesCount;
        private TextBox tbGigabytesCount;
        private ProgressBar progressBar1;
        private Label lblStatus;
    }
}
