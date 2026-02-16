using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskScheduler;

namespace NoCopilotKey_Installer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.pictureBox1.Image = System.Drawing.SystemIcons.Shield.ToBitmap();
            this.pictureBox1.Left = this.optProgramFiles.Left + this.optProgramFiles.Width + 0;
            this.pictureBox1.Top = this.optProgramFiles.Top + (this.optProgramFiles.Height - this.pictureBox1.Height) / 2;
            this.uninstallButton.Enabled = Installer.CanUninstall();
        }

        static bool IsInstalledToProgramFiles()
        {
            string programFilesDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string noCopilotKeyDirectory = Path.Combine(programFilesDirectory, "NoCopilotKey");
            if (Directory.Exists(noCopilotKeyDirectory))
            {
                return File.Exists(Path.Combine(noCopilotKeyDirectory, "NoCopilotKey.exe")) ||
                    File.Exists(Path.Combine(noCopilotKeyDirectory, "NoCopilotKey Installer.exe"));
            }
            return false;
        }

        private void uninstallButton_Click(object sender, EventArgs e)
        {
            Uninstall();
        }

        void Uninstall()
        {
            Installer.LaunchInstaller(new string[] { "--uninstall" });
            Environment.Exit(0);
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            Install();
        }

        void Install()
        {
            List<string> args = new List<string>();
            if (this.optProgramFiles.Checked)
            {
                args.Add("--install-to-program-files");
                args.Add("--register-as-scheduled-task");
            }
            else if (this.optUserProgramFiles.Checked)
            {
                args.Add("--install-to-user-program-files");
                args.Add("--register-as-startup-item");
            }
            var process = Installer.LaunchInstaller2(args.ToArray());
            int exitCode = 1;
            if (process != null)
            {
                this.Enabled = false;
                while (!process.WaitForExit(10))
                {
                    Application.DoEvents();
                }
                exitCode = process.ExitCode;
                this.Enabled = true;

            }
            if (exitCode == 0)
            {
                MessageBox.Show(this, "Installation Successful", "NoCopilotKey", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, "Installation Failed", "NoCopilotKey", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            this.uninstallButton.Enabled = Installer.CanUninstall();
        }
    }
}
