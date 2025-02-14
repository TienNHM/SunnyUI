using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Sunny.UI
{
    public static class ApplicationEx
    {
        private static readonly string StartUpPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        /// <summary>
        /// Get the GUID of the current application
        /// </summary>
        /// <returns>Application GUID</returns>
        public static Guid AppGuid()
        {
            var asm = Assembly.GetEntryAssembly();
            if (asm == null) return Guid.Empty;
            object[] attr = (asm.GetCustomAttributes(typeof(GuidAttribute), true));
            return attr.Length > 0 && attr[0] is GuidAttribute ga ? new Guid(ga.Value) : Guid.Empty;
        }

        /// <summary>
        /// Get the path of the specified special folder and process the path format
        /// </summary>
        /// <param name="specialFolder">Special folder enumeration</param>
        /// <returns>Processed folder path</returns>
        public static string Folder(this Environment.SpecialFolder specialFolder) => Environment.GetFolderPath(specialFolder).DealPath();

        /// <summary>
        /// Get the application folder path under the specified special folder and create the folder if necessary
        /// </summary>
        /// <param name="specialFolder">Special folder enumeration</param>
        /// <param name="createIfNotExists">Whether to create the folder if it does not exist</param>
        /// <returns>Application folder path</returns>
        public static string FolderWithApplication(this Environment.SpecialFolder specialFolder, bool createIfNotExists = true)
        {
            var dir = (specialFolder.Folder() + Application.ProductName).DealPath();
            if (createIfNotExists) Dir.CreateDir(dir);
            return dir;
        }

        /// <summary>
        /// Get the directory for the common repository of application-specific data for the current roaming user
        /// C:\Users\{YourUserName}\AppData\Roaming\{Application.ProductName}\
        /// </summary>
        /// <returns>Application data folder path</returns>
        public static string ApplicationDataFolder() => Environment.SpecialFolder.ApplicationData.FolderWithApplication();

        /// <summary>
        /// Get the directory for the common repository of application-specific data for the current non-roaming user
        /// C:\Users\{YourUserName}\AppData\Local\{Application.ProductName}\
        /// </summary>
        /// <returns>Local application data folder path</returns>
        public static string LocalApplicationDataFolder() => Environment.SpecialFolder.LocalApplicationData.FolderWithApplication();

        /// <summary>
        /// Get the directory for the common repository of application-specific data for all users
        /// C:\ProgramData \{Application.ProductName}\
        /// </summary>
        /// <returns>Common application data folder path</returns>
        public static string CommonApplicationDataFolder() => Environment.SpecialFolder.CommonApplicationData.FolderWithApplication();

        /// <summary>
        /// Add the current program to startup
        /// </summary>
        /// <param name="arguments">Startup arguments</param>
        public static void AddToStartup(string arguments)
        {
            using var key = Registry.CurrentUser.OpenSubKey(StartUpPath, true);
            key?.SetValue(Application.ProductName, $"\"{Application.ExecutablePath}\" {arguments}");
        }

        /// <summary>
        /// Add the current program to startup
        /// </summary>
        public static void AddToStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(StartUpPath, true);
            key?.SetValue(Application.ProductName, $"\"{Application.ExecutablePath}\"");
        }

        /// <summary>
        /// Remove the current program from startup
        /// </summary>
        public static void RemoveFromStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(StartUpPath, true);
            if (Application.ProductName != null) key?.DeleteValue(Application.ProductName, false);
        }

        /// <summary>
        /// Determine whether the current program is set to run at startup
        /// </summary>
        /// <returns>Whether the program is set to run at startup</returns>
        public static bool StartupEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(StartUpPath, false);
            return key != null && key.GetValue(Application.ProductName) != null;
        }

        /// <summary>
        /// Check and update the startup path of the current program
        /// </summary>
        public static void CheckAndUpdateStartupPath()
        {
            if (!StartupEnabled()) return;

            string arg = string.Empty;
            // Read startup arguments from the registry key value
            using var key = Registry.CurrentUser.OpenSubKey(StartUpPath, true);
            if (key == null) return;
            string oldValue = key.GetValue(Application.ProductName)?.ToString();
            if (oldValue == null) return;
            if (oldValue.StartsWith("\""))
            {
                arg = string.Join("\"", oldValue.Split('\"').Skip(2)).Trim();
            }
            else if (oldValue.Contains(" "))
            {
                arg = string.Join(" ", oldValue.Split(' ').Skip(1));
            }

            if (string.IsNullOrEmpty(arg))
                AddToStartup();
            else
                AddToStartup(arg);
        }
    }
}
