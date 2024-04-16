using Google.Protobuf;
using Grpc.Core;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;

namespace ServiceUpdate1.GrpcServer.Services
{
    public class DeployUpdatesService : GrpcServer.DeployUpdatesService.DeployUpdatesServiceBase
    {
        private readonly string _currentVersion = "1.1.0"; // Example: Simulate a newer version
        private  string _filePath = @"C:\Windows\regedit.exe"; // Example: Simulate a newer version
        private string _updateInstallerFilePath = @"C:\Update Installer\Updateinstaller.exe";

        public override Task<VersionInfo> GetLatestVersion(Empty request, ServerCallContext context)
        {
            if(!File.Exists(_filePath))
            {
                const string msg = "Service is not installed or not at specified location";
                throw new RpcException(new Status(StatusCode.FailedPrecondition , msg));
            }
            var versionInfo = FileVersionInfo.GetVersionInfo(_filePath);
            //string version = versionInfo.FileVersion;
            return Task.FromResult(new VersionInfo { Version = versionInfo.ProductVersion  });
        }
        public override Task<ResponseMessage> SendUpdates(FileMessage message, ServerCallContext context)
        {
            //string outputFilePath = AppContext.BaseDirectory + $"ServiceUpdate1.GrpcClient{counter}.exe";
            try
            {
                ByteString byteString = message.Content;
                if(!Directory.Exists(_updateInstallerFilePath))
                    Directory.CreateDirectory(Path.GetDirectoryName(_updateInstallerFilePath));
                using (FileStream outputStream = File.OpenWrite(_updateInstallerFilePath))
                {
                    byteString.WriteTo(outputStream);
                    _filePath = _updateInstallerFilePath;
                }
                return Task.FromResult(new ResponseMessage { Message = "SUCCESS" });
            }
            catch (Exception)
            {
                return Task.FromResult(new ResponseMessage { Message = "UNSUCCESS" });
            }
        }

        public override Task<ResponseMessage> InstallUpdates(Empty request, ServerCallContext context)
        {
            try
            {
                //try to install updates using _updateInstallerFilePath  file
                return Task.FromResult(new ResponseMessage { Message = "SUCCESS" });
            }
            catch (Exception)
            {
                return Task.FromResult(new ResponseMessage { Message = "UNSUCCESS" });
            }
        }

    }
}
