using Grpc.Core;
using System;
using System.Configuration;
using System.Net;
using System.ServiceProcess;

namespace ServiceUpdate1.ConsoleServiceHost
{
    public class MyWcfService : ServiceBase
    {
        private System.Threading.Timer _updateTimer;
        private string _currentVersion;
        private Channel _grpcChannel;

        public MyWcfService()
        {
            ServiceName = "MyWcfService";
        }

        protected override void OnStart(string[] args)
        {
            // Load service version from configuration
            _currentVersion = ConfigurationManager.AppSettings["version"];

            // Initialize gRPC channel to update server
            _grpcChannel = new Channel("localhost:50051", ChannelCredentials.Insecure);

            // Start WCF service implementation
            // (Replace with your actual WCF service logic)
            StartWcfService();

            // Schedule update check
            _updateTimer = new System.Threading.Timer(CheckForUpdates, null, TimeSpan.Zero, TimeSpan.FromHours(1)); // Check every hour
        }

        protected override void OnStop()
        {
            _updateTimer.Dispose();
            _grpcChannel.ShutdownAsync().Wait();
            // Stop WCF service implementation
            StopWcfService();
        }

        private void StartWcfService()
        {
            // Implement your actual WCF service logic to start here
            Console.WriteLine("WCF Service started (version: " + _currentVersion + ")");
        }

        private void StopWcfService()
        {
            // Implement your actual WCF service logic to stop here
            Console.WriteLine("WCF Service stopped");
        }

        private async void CheckForUpdates(object state)
        {
            try
            {
                var client = new UpdateService.UpdateServiceClient(_grpcChannel);
                var versionInfo = await client.GetLatestVersionAsync(new Empty());

                if (versionInfo.Version != _currentVersion)
                {
                    Console.WriteLine("Newer version available: " + versionInfo.Version);

                    // Download logic (optional)
                    if (!string.IsNullOrEmpty(versionInfo.DownloadUrl))
                    {
                        DownloadUpdate(versionInfo.DownloadUrl);
                    }

                    UpdateServiceConfiguration(versionInfo);
                    ScheduleServiceRestart();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking for updates: " + ex.Message);
            }
        }

        private void DownloadUpdate(string downloadUrl)
        {
            try
            {
                // Implement download logic using WebClient, HttpClient, etc.
                // Download the update package from the provided URL
                // (This is a placeholder, replace with your specific implementation)
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(downloadUrl, "update.package");
                    Console.WriteLine("Update downloaded successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error downloading update: " + ex.Message);
            }
        }

        private void UpdateServiceConfiguration(VersionInfo newVersion)
        {
            var config = ConfigurationManager.OpenMappedExeConfiguration(
                ConfigurationUserLevel.PerUserRoaming, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            config.AppSettings.Settings["version"].Value = newVersion.Version;
            config.Save();
            ConfigurationManager.RefreshSection("service");

            _currentVersion = newVersion.Version;
        }

        private void ScheduleServiceRestart()
        {
            // You can implement various restart mechanisms here
            // Option 1: Use Windows Task Scheduler to restart the service at a specific time
            // Option 2: Trigger a service restart command after a delay

            Console.WriteLine("Service restart scheduled for update...");
        }
    }
}