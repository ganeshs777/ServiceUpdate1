using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
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
        private string _uploadFolderPath = @"C:\Update Installer";

        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public DeployUpdatesService(ILoggerFactory loggerFactory, IConfiguration config)
        {
            _logger = loggerFactory.CreateLogger<DeployUpdatesService>();
            _config = config;
        }

        //public DeployUpdatesService(ILogger logger, string uploadPath)
        //{
        //    _logger = logger;
        //    _uploadFolderPath = uploadPath;
        //}

        public override Task<VersionInfo> GetLatestVersion(Empty request, ServerCallContext context)
        {
            if (!File.Exists(_filePath))
            {
                const string msg = "Service is not installed or not at specified location";
                _logger.LogError(msg, DateTimeOffset.Now);
                throw new RpcException(new Status(StatusCode.FailedPrecondition, msg));
            }
            var versionInfo = FileVersionInfo.GetVersionInfo(_filePath);
            _logger.LogInformation($"{DateTimeOffset.Now} : File : {_filePath} Version: {versionInfo.ProductVersion} ", DateTimeOffset.Now);
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
            //var uploadPath = Path.Combine(_config["StoredFilesPath"]!, uploadId);
            var uploadPath = Path.Combine(_uploadFolderPath , uploadId);

            Directory.CreateDirectory(uploadPath);

            await using var writeStream = File.Create(Path.Combine(uploadPath, "data.bin"));

            await foreach (var message in requestStream.ReadAllAsync())
            {
                if (message.Metadata != null)
                {
                    await File.WriteAllTextAsync(Path.Combine(uploadPath, "metadata.json"), message.Metadata.ToString());
                }
                if (message.Data != null)
                {
                    await writeStream.WriteAsync(message.Data.Memory);
                }
            }

            return new UploadFileResponse { Id = uploadId };
        }

        public override Task<ResponseMessage> XCopy(FileMessage message, ServerCallContext context)
        {
            //string outputFilePath = AppContext.BaseDirectory + $"ServiceUpdate1.GrpcClient{counter}.exe";
            try
            {
                if (!Directory.Exists(_updateInstallerFolderPath))
                    Directory.CreateDirectory(_updateInstallerFolderPath);
                _filePath = Path.Combine(_updateInstallerFolderPath, message.Filename);
                ProcessXcopy(message.ContentPath, _updateInstallerFolderPath);
                return Task.FromResult(new ResponseMessage { Message = "SUCCESS" });
            }
            catch (Exception)
            {
                return Task.FromResult(new ResponseMessage { Message = "UNSUCCESS" });
            }
        }

        /// <summary>
        /// Method to Perform Xcopy to copy files/folders from Source machine to Target Machine
        /// </summary>
        /// <param name="SolutionDirectory"></param>
        /// <param name="TargetDirectory"></param>
        private void ProcessXcopy(string SolutionDirectory, string TargetDirectory)
        {
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            //Give the name as Xcopy
            startInfo.FileName = "xcopy";
            //make the window Hidden
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //Send the Source and destination as Arguments to the process
            startInfo.Arguments = "\"" + SolutionDirectory + "\"" + " " + "\"" + TargetDirectory + "\"" + @" /e /y /I";
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }

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
