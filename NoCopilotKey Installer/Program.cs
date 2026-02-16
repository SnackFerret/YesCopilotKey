using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoCopilotKey_Installer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            var args = Environment.GetCommandLineArgs();
            Main2(args);
        }
        public static void Main2(string[] args)
        {
            InstallationMode installationMode = InstallationMode.Undefined;
            AutoRunMode autoRunMode = AutoRunMode.Undefined;

            bool doUninstall = false;
            bool doStop = false;

            if (args.Contains("--install-to-program-files"))
            {
                installationMode = InstallationMode.InstallToProgramFiles;
            }
            else if (args.Contains("--install-to-user-program-files"))
            {
                installationMode = InstallationMode.InstallToUserProgramFiles;
            }
            else if (args.Contains("--leave-exe-here"))
            {
                installationMode = InstallationMode.LeaveExeHere;
            }

            if (args.Contains("--register-as-scheduled-task"))
            {
                autoRunMode = AutoRunMode.ScheduledTask;
            }
            else if (args.Contains("--register-as-startup-item"))
            {
                autoRunMode = AutoRunMode.StartupItem;
            }
            else if (args.Contains("--no-auto-run"))
            {
                autoRunMode = AutoRunMode.NoAutoRun;
            }

            if (args.Contains("--uninstall"))
            {
                doUninstall = true;
            }
            if (args.Contains("--stop"))
            {
                doStop = true;
            }

            if (!doUninstall && !doStop && installationMode != InstallationMode.Undefined && autoRunMode != AutoRunMode.Undefined)
            {
                bool installOkay = Installer.Install(installationMode, autoRunMode);
                if (!installOkay) Environment.Exit(1);
                return;
            }

            if (doUninstall)
            {
                Installer.Uninstall();
                return;
            }

            if (doStop)
            {
                Installer.StopProgram();
                return;
            }

            var currentProcess = Process.GetCurrentProcess();
            {
                var instances = Process.GetProcessesByName(currentProcess.ProcessName);
                foreach (var instance in instances)
                {
                    if (instance.Id != currentProcess.Id && instance.MainWindowHandle != IntPtr.Zero && 
                        Application.ExecutablePath.Equals(Installer.GetProcessFullName(instance), StringComparison.OrdinalIgnoreCase))
                    {
                        SetForegroundWindow(instance.MainWindowHandle);
                        Environment.Exit(1);
                    }
                }
                foreach (var instance in instances)
                {
                    instance.Dispose();
                }
            }



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }


        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
    }

}
