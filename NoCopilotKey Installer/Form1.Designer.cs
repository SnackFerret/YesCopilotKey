
namespace NoCopilotKey_Installer
{
    partial class Form2
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
            this.optProgramFiles = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.optUserProgramFiles = new System.Windows.Forms.RadioButton();
            this.uninstallButton = new System.Windows.Forms.Button();
            this.installButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // optProgramFiles
            // 
            this.optProgramFiles.AutoSize = true;
            this.optProgramFiles.Checked = true;
            this.optProgramFiles.Location = new System.Drawing.Point(6, 19);
            this.optProgramFiles.Name = "optProgramFiles";
            this.optProgramFiles.Size = new System.Drawing.Size(159, 17);
            this.optProgramFiles.TabIndex = 0;
            this.optProgramFiles.TabStop = true;
            this.optProgramFiles.Text = "Install to run as Administrator";
            this.optProgramFiles.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.optUserProgramFiles);
            this.groupBox1.Controls.Add(this.optProgramFiles);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(186, 67);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Install Mode:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(163, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // optUserProgramFiles
            // 
            this.optUserProgramFiles.AutoSize = true;
            this.optUserProgramFiles.Location = new System.Drawing.Point(6, 42);
            this.optUserProgramFiles.Name = "optUserProgramFiles";
            this.optUserProgramFiles.Size = new System.Drawing.Size(154, 17);
            this.optUserProgramFiles.TabIndex = 1;
            this.optUserProgramFiles.Text = "Install to run as regular user";
            this.optUserProgramFiles.UseVisualStyleBackColor = true;
            // 
            // uninstallButton
            // 
            this.uninstallButton.Location = new System.Drawing.Point(44, 146);
            this.uninstallButton.Name = "uninstallButton";
            this.uninstallButton.Size = new System.Drawing.Size(75, 23);
            this.uninstallButton.TabIndex = 1;
            this.uninstallButton.Text = "Uninstall";
            this.uninstallButton.UseVisualStyleBackColor = true;
            this.uninstallButton.Click += new System.EventHandler(this.uninstallButton_Click);
            // 
            // installButton
            // 
            this.installButton.Location = new System.Drawing.Point(125, 146);
            this.installButton.Name = "installButton";
            this.installButton.Size = new System.Drawing.Size(75, 23);
            this.installButton.TabIndex = 2;
            this.installButton.Text = "Install";
            this.installButton.UseVisualStyleBackColor = true;
            this.installButton.Click += new System.EventHandler(this.installButton_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 57);
            this.label1.TabIndex = 3;
            this.label1.Text = "To support applications which run as Administrator, NoCopilotKey must be installe" +
    "d to run as Administrator.\r\n";
            // 
            // Form2
            // 
            this.AcceptButton = this.installButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(212, 181);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.installButton);
            this.Controls.Add(this.uninstallButton);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form2";
            this.Text = "NoCopilotKey Installer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton optProgramFiles;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RadioButton optUserProgramFiles;
        private System.Windows.Forms.Button uninstallButton;
        private System.Windows.Forms.Button installButton;
        private System.Windows.Forms.Label label1;
    }
}