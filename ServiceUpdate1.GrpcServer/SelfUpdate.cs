using System.Diagnostics;
using System.IO.Compression;
using System.Net;

namespace ServiceUpdate1.GrpcServer
{
    public class SelfUpdate
    {
        private readonly string TempZipPath = "C:\\Temp\\update.zip";

        public string VersionUrl { get; private set; }
        public string UpdateUrl { get; private set; }
        public string ApplicationDirectory { get; private set; }

        public SelfUpdate(string versionURL, string updateUrl, String applicationDirectory)
        {
            VersionUrl = versionURL;
            UpdateUrl = updateUrl;
            ApplicationDirectory = applicationDirectory;
        }
        public void CheckForUpdate()
        {
            try
            {
                string currentVersion = GetInstalledVersion();
                string latestVersion = GetLatestVersion();

                if (latestVersion != null && latestVersion != currentVersion)
                {
                    DownloadUpdate();
                    ReplaceFiles();
                    RestartApplication();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking for updates: " + ex.Message);
            }
        }

        private string GetInstalledVersion()
        {
            // Logic to get the current version of the installed application
            return "1.0";
        }

        private string GetLatestVersion()
        {
            // Logic to download and parse the version file from the server
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(VersionUrl);
            }
        }

        private void DownloadUpdate()
        {
            // Logic to download the update files to a temporary directory
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(UpdateUrl, TempZipPath);
            }
        }

        private void ReplaceFiles()
        {
            // Logic to replace the existing files with the downloaded update files
            // You may need to handle file locking and permissions here
            var extractPath = Path.Combine(ApplicationDirectory + "update");
            //File.Copy(TempZipPath, extractPath, true);
            ZipFile.ExtractToDirectory(TempZipPath, extractPath, true);
            // Extract the zip file and replace/update files as needed
        }

        private void RestartApplication()
        {
            // Logic to restart the application
            Thread.Sleep(2000); // Wait for the file operations to complete
            Process.Start(Path.Combine(ApplicationDirectory + "ServiceUpdate1.GrpcServer.exe"));
            Environment.Exit(0); // Exit the current instance of the application
        }
    }
}
