using Google.Protobuf;
using Grpc.Core;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Net;

namespace ServiceUpdate1.GrpcServer.Services
{
    public class DeployUpdatesService : GrpcServer.DeployUpdatesService.DeployUpdatesServiceBase
    {
        private readonly string _currentVersion = "1.1.0"; // Example: Simulate a newer version
        private string _filePath = @"C:\Windows\regedit.exe"; // Example: Simulate a newer version
        private string _updateInstallerFolderPath = @"C:\Update Installer";

        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public DeployUpdatesService(ILoggerFactory loggerFactory, IConfiguration config)
        {
            _logger = loggerFactory.CreateLogger<DeployUpdatesService>();
            _config = config;
        }

        public override Task<VersionInfo> GetLatestVersion(Empty request, ServerCallContext context)
        {
            if (!File.Exists(_filePath))
            {
                const string msg = "Service is not installed or not at specified location";
                throw new RpcException(new Status(StatusCode.FailedPrecondition, msg));
            }
            var versionInfo = FileVersionInfo.GetVersionInfo(_filePath);
            //string version = versionInfo.FileVersion;
            return Task.FromResult(new VersionInfo { Version = versionInfo.ProductVersion });
        }
        public override Task<ResponseMessage> SendUpdates(FileMessage message, ServerCallContext context)
        {
            //string outputFilePath = AppContext.BaseDirectory + $"ServiceUpdate1.GrpcClient{counter}.exe";
            try
            {
                ByteString byteString = message.Content;
                if (!Directory.Exists(_updateInstallerFolderPath))
                    Directory.CreateDirectory(_updateInstallerFolderPath);
                _filePath = Path.Combine(_updateInstallerFolderPath, message.Filename);
                using (FileStream outputStream = File.OpenWrite(_filePath))
                {
                    byteString.WriteTo(outputStream);
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

        public override async Task<UploadFileResponse> UploadFile(IAsyncStreamReader<UploadFileRequest> requestStream, ServerCallContext context)
        {
            var uploadId = Path.GetRandomFileName();
            var uploadPath = Path.Combine(_config["StoredFilesPath"]!, uploadId);
            Directory.CreateDirectory(uploadPath);

            await using var writeStream = File.Create(Path.Combine(uploadPath, "data.bin"));
            var SourceFolderPath = "";
            var TargetFolderPath = "";
            var FileName = "";
            await foreach (var message in requestStream.ReadAllAsync())
            {
                if (message.Metadata != null)
                {
                    FileName = message.Metadata.FileName;
                    SourceFolderPath = message.Metadata.SourceFolderPath;
                    TargetFolderPath = message.Metadata.TargetFolderPath;
                    await File.WriteAllTextAsync(Path.Combine(uploadPath, "metadata.json"), message.Metadata.ToString());
                }
                if (message.Data != null)
                {
                    await writeStream.WriteAsync(message.Data.Memory);
                }
            }
            if (!Directory.Exists(TargetFolderPath))
                Directory.CreateDirectory(TargetFolderPath);
            writeStream.Close();
            File.Move(Path.Combine(uploadPath, "data.bin"), Path.Combine(TargetFolderPath, FileName), true);
            return new UploadFileResponse { Id = uploadId };
        }
    }

    //public class UpdateServer
    //{
    //    public static void Main(string[] args)
    //    {
    //        var server = new Server
    //        {
    //            Services = { GrpcServer.DeployUpdatesService.BindService(new DeployUpdatesService()) },
    //            Ports = { new ServerPort("10.5.92.167", 5000, ServerCredentials.Insecure) }
    //        };
    //        server.Start();
    //        Console.WriteLine("Update server listening on port 50051");
    //        Console.ReadLine();
    //    }
    //}
}
