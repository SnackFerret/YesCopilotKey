using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoCopilotKey_Installer
{
    public enum InstallationMode
    {
        Undefined = 0,
        LeaveExeHere,
        InstallToProgramFiles,
        InstallToUserProgramFiles,
    }

    public enum AutoRunMode
    {
        Undefined = 0,
        NoAutoRun,
        ScheduledTask,
        StartupItem,
    }

    public static class Installer
    {
        public static string GetProgramFilesAppDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "NoCopilotKey");
        }

        public static string GetUserProgramFilesAppDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "NoCopilotKey");
        }

        public static string GetStartupShortcutPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "NoCopilotKey.lnk");
        }

        public static bool Install(InstallationMode installationMode, AutoRunMode autoRunMode)
        {
            bool needAdmin = installationMode == InstallationMode.InstallToProgramFiles || autoRunMode == AutoRunMode.ScheduledTask;
            if (!IsAdmin() && needAdmin)
            {
                RestartAsAdmin();
            }

            string targetDirectory = "";
            if (installationMode == InstallationMode.InstallToProgramFiles)
            {
                targetDirectory = GetProgramFilesAppDirectory();
            }
            else if (installationMode == InstallationMode.InstallToUserProgramFiles)
            {
                targetDirectory = GetUserProgramFilesAppDirectory();
            }
            else if (installationMode == InstallationMode.LeaveExeHere)
            {
                targetDirectory = Application.StartupPath;
            }
            else
            {
                throw new InvalidOperationException();
            }
            string exePath = Path.Combine(targetDirectory, "NoCopilotKey.exe");
            string installerExePath = Path.Combine(targetDirectory, "NoCopilotKey Installer.exe");

            Directory.CreateDirectory(targetDirectory);
            bool exeOkay = ExtractExe(exePath);
            if (!exeOkay) return false;
            if (!installerExePath.Equals(Application.ExecutablePath, StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(Application.ExecutablePath, installerExePath, true);
            }

            if (autoRunMode == AutoRunMode.ScheduledTask)
            {
                ScheduledTask.CreateScheduledTask(exePath, "NoCopilotKey", "Dan Weiss (www.dwedit.org)", "Changes Copilot keyboard key into right ctrl key");
            }
            else if (autoRunMode == AutoRunMode.StartupItem)
            {
                string lnkFileName = GetStartupShortcutPath();
                Shortcut.CreateShortcut(lnkFileName, exePath);
            }

            string registryPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\NoCopilotKey";
            Microsoft.Win32.RegistryKey subkey = null;
            if (installationMode == InstallationMode.InstallToProgramFiles)
            {
                subkey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(registryPath);
            }
            else if (installationMode == InstallationMode.InstallToUserProgramFiles)
            {
                subkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(registryPath);
            }
            if (subkey != null)
            {
                subkey.SetValue("DisplayName", "NoCopilotKey");
                //subkey.SetValue("DisplayVersion", "1.0.1.0");
                subkey.SetValue("Publisher", "www.dwedit.org");
                subkey.SetValue("URLInfoAbout", "https://github.com/Dwedit/NoCopilotKey");
                subkey.SetValue("UninstallString", "\"" + installerExePath + "\" --uninstall");
            }

            StopProgram();
            Process.Start(exePath);
            return true;
        }

        public static bool ExtractExe(string targetPath)
        {
            try
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NoCopilotKey_Installer.NoCopilotKey.exe");
                BinaryReader br = new BinaryReader(stream);
                byte[] bytes = br.ReadBytes((int)stream.Length);
                File.WriteAllBytes(targetPath, bytes);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool CanUninstall()
        {
            return IsInstalledToProgramFiles() || IsInstalledToUserProgramFiles() || IsScheduledTask() || IsStartupItem();
        }

        static bool TryDeleteFile(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    return true;
                }
                File.Delete(fileName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static bool TryDeleteDirectory(string directoryName)
        {
            try
            {
                if (!Directory.Exists(directoryName))
                {
                    return true;
                }
                Directory.Delete(directoryName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void Uninstall()
        {
            bool isInstalledToProgramFiles = IsInstalledToProgramFiles();
            bool isScheduledTask = IsScheduledTask();

            if ((isInstalledToProgramFiles || isScheduledTask) && !IsAdmin())
            {
                RestartAsAdmin();
            }
            List<string> filesToDelete = new List<string>();
            List<string> directoriesToDelete = new List<string>();
            bool TryDeleteFile2(string fileNameToDelete)
            {
                if (File.Exists(fileNameToDelete))
                {
                    bool deleted = TryDeleteFile(fileNameToDelete);
                    if (!deleted)
                    {
                        filesToDelete.Add(fileNameToDelete);
                    }
                    return deleted;
                }
                return true;
            }
            bool TryDeleteDirectory2(string directoryNameToDelete)
            {
                if (Directory.Exists(directoryNameToDelete))
                {
                    bool deleted = TryDeleteDirectory(directoryNameToDelete);
                    if (!deleted)
                    {
                        directoriesToDelete.Add(directoryNameToDelete);
                    }
                    return deleted;
                }
                return true;
            }


            //bool deleteSelfAndDirectory = false;
            //string deleteFileName1 = null;
            //string deleteDirectory1 = null;

            //StopProgram();
            if (IsInstalledToProgramFiles())
            {
                string programDirectory = GetProgramFilesAppDirectory();
                string exeName = Path.Combine(programDirectory, "NoCopilotKey.exe");
                string installerExeName = Path.Combine(programDirectory, "NoCopilotKey Installer.exe");
                string installerExeName2 = Path.Combine(programDirectory, "NoCopilotKey Installer.pdb");
                string installerExeName3 = Path.Combine(programDirectory, "NoCopilotKey Installer.exe.config");
                StopProgram(exeName);
                bool deletedExe = TryDeleteFile2(exeName);
                bool deletedInstaller1 = TryDeleteFile2(installerExeName);
                bool deletedInstaller2 = TryDeleteFile2(installerExeName2);
                bool deletedInstaller3 = TryDeleteFile2(installerExeName3);
                bool deletedDirectory = TryDeleteDirectory2(programDirectory);
            }
            if (IsInstalledToUserProgramFiles())
            {
                string programDirectory = GetUserProgramFilesAppDirectory();
                string exeName = Path.Combine(programDirectory, "NoCopilotKey.exe");
                string installerExeName = Path.Combine(programDirectory, "NoCopilotKey Installer.exe");
                string installerExeName2 = Path.Combine(programDirectory, "NoCopilotKey Installer.pdb");
                string installerExeName3 = Path.Combine(programDirectory, "NoCopilotKey Installer.exe.config");
                StopProgram(exeName);
                bool deletedExe = TryDeleteFile2(exeName);
                bool deletedInstaller1 = TryDeleteFile2(installerExeName);
                bool deletedInstaller2 = TryDeleteFile2(installerExeName2);
                bool deletedInstaller3 = TryDeleteFile2(installerExeName3);
                bool deletedDirectory = TryDeleteDirectory2(programDirectory);
            }
            if (IsScheduledTask())
            {
                ScheduledTask.RemoveScheduledTask("NoCopilotKey");
            }
            if (IsStartupItem())
            {
                string startupLnk = GetStartupShortcutPath();
                bool deletedShortcut = TryDeleteFile(startupLnk);
            }
            //remove uninstaller from registry
            string registryPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\NoCopilotKey";
            Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(registryPath, false);
            Microsoft.Win32.Registry.LocalMachine.DeleteSubKeyTree(registryPath, false);

            if (filesToDelete.Count > 0 || directoriesToDelete.Count > 0)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "cmd.exe"));
                startInfo.Arguments = "/C sleep 0.5";
                foreach (var deleteFileName in filesToDelete)
                {
                    startInfo.Arguments += " & del \"" + deleteFileName + "\"";
                }
                foreach (var deleteDirectory in directoriesToDelete)
                {
                    startInfo.Arguments += " & rmdir \"" + deleteDirectory + "\"";
                }
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                Process.Start(startInfo);
                Environment.Exit(0);
            }
        }

        public static bool IsInstalledToDirectory(string directoryName)
        {
            if (Directory.Exists(directoryName))
            {
                string noCopilotKeyExe = Path.Combine(directoryName, "NoCopilotKey.exe");
                string noCopilotKeyInstallerExe = Path.Combine(directoryName, "NoCopilotKey Installer.exe");
                if (File.Exists(noCopilotKeyExe)) return true;
                if (File.Exists(noCopilotKeyInstallerExe)) return true;
                if (Directory.EnumerateFileSystemEntries(directoryName).FirstOrDefault() != null) return false;
                return true;
            }
            return false;
        }

        public static bool IsInstalledToProgramFiles()
        {
            return IsInstalledToDirectory(GetProgramFilesAppDirectory());
        }
        public static bool IsInstalledToUserProgramFiles()
        {
            return IsInstalledToDirectory(GetUserProgramFilesAppDirectory());
        }
        public static bool IsScheduledTask()
        {
            var scheduledTask = ScheduledTask.GetScheduledTask("NoCopilotKey");
            return scheduledTask != null;
        }
        public static bool IsStartupItem()
        {
            return File.Exists(GetStartupShortcutPath());
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, ref int lpdwSize);

        public static string GetProcessFullName(Process process)
        {
            int capacity = 1024;
            StringBuilder sb = new StringBuilder(capacity);
            QueryFullProcessImageName(process.Handle, 0, sb, ref capacity);
            return sb.ToString();
        }

        public static void StopProgram(string exePath)
        {
            var processes = Process.GetProcessesByName("NoCopilotKey");
            foreach (var process in processes)
            {
                string exeName = GetProcessFullName(process);
                if (exePath.Equals(exeName, StringComparison.OrdinalIgnoreCase))
                {
                    process.Kill();
                }
            }
        }

        public static void StopProgram()
        {
            var processes = Process.GetProcessesByName("NoCopilotKey");
            foreach (var process in processes)
            {
                //string exeName = GetProcessFullName(process);
                process.Kill();
            }
        }

        public static bool LaunchAsAdmin(string[] args = null)
        {
            return LaunchInstaller2(args, true) != null;
        }

        public static bool LaunchInstaller(string[] args = null, bool admin = false)
        {
            var process = LaunchInstaller2(args, admin);
            return process != null;

            //if (args == null)
            //{
            //    args = Environment.GetCommandLineArgs().Skip(1).ToArray();
            //}
            //var startInfo = new ProcessStartInfo(Application.ExecutablePath);
            //startInfo.Arguments = string.Join(" ", args);
            //if (admin)
            //{
            //    startInfo.Verb = "runas";
            //}
            //startInfo.UseShellExecute = true;
            //try
            //{
            //    Process.Start(startInfo);
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            //return true;
        }

        public static Process LaunchInstaller2(string[] args = null, bool admin = false)
        {
            if (args == null)
            {
                args = Environment.GetCommandLineArgs().Skip(1).ToArray();
            }

            var startInfo = new ProcessStartInfo(Application.ExecutablePath);
            startInfo.Arguments = string.Join(" ", args);
            if (admin)
            {
                startInfo.Verb = "runas";
            }
            startInfo.UseShellExecute = true;
            Process otherProcess = null;
            try
            {
                otherProcess = Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                return null;
            }
            return otherProcess;
        }

        public static void RestartAsAdmin(string[] args = null)
        {
            bool success = LaunchAsAdmin(args);
            if (!success) Environment.Exit(1);
            Environment.Exit(0);
        }

        public static bool IsAdmin()
        {
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }
}
