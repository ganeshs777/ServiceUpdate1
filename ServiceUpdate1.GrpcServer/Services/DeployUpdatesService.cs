using Google.Protobuf;
using Grpc.Core;
using System.Diagnostics;

namespace ServiceUpdate1.GrpcServer.Services
{
    public class DeployUpdatesService : GrpcServer.DeployUpdatesService.DeployUpdatesServiceBase
    {
        private string _filePath = "";
        private string _updateInstallerFolderPath = @"C:\Update Installer";
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public DeployUpdatesService(ILoggerFactory loggerFactory, IConfiguration config)
        {
            _logger = loggerFactory.CreateLogger<DeployUpdatesService>();
            _config = config;
        }

        /// <summary>
        /// Verify specified location and reply with version information
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override Task<ResponseMessage> GetLatestVersion(Empty request, ServerCallContext context)
        {
            var filePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var versionInfo = FileVersionInfo.GetVersionInfo(filePath);
            return ReturnResult(true, versionInfo.ProductVersion, "");
        }

        /// <summary>
        /// Sending updated file through gRPC
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ResponseMessage> SendUpdates(FileMessage message, ServerCallContext context)
        {
            //string outputFilePath = AppContext.BaseDirectory + $"ServiceUpdate1.GrpcClient{counter}.exe";
            try
            {
                ByteString byteString = message.Content;
                _updateInstallerFolderPath = message.TargetFolderPath;
                if (!Directory.Exists(_updateInstallerFolderPath))
                    Directory.CreateDirectory(_updateInstallerFolderPath);
                _filePath = Path.Combine(_updateInstallerFolderPath, message.Filename);
                using (FileStream outputStream = File.OpenWrite(_filePath))
                {
                    byteString.WriteTo(outputStream);
                }
                return ReturnResult(true, _filePath, "");
            }
            catch (Exception ex)
            {
                return ReturnResult(false, _filePath, ex.Message);
            }
        }

        /// <summary>
        /// It will install required updates
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ResponseMessage> InstallUpdates(Empty request, ServerCallContext context)
        {
            try
            {
                //TO-DO - try to install updates using _updateInstallerFilePath  file
                return ReturnResult(true, _filePath, "");
            }
            catch (Exception ex)
            {
                return ReturnResult(false, _filePath, ex.Message);
            }
        }

        /// <summary>
        /// Upload large files with gRPC using chunks
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ResponseMessage> UploadFile(IAsyncStreamReader<UploadFileRequest> requestStream, ServerCallContext context)
        {
            try
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
                return ReturnResult(true, Path.Combine(TargetFolderPath, FileName), "").Result;
            }
            catch (Exception ex)
            {
                return ReturnResult(false, "Error", ex.Message).Result;
            }
        }

        /// <summary>
        /// Copy specified source folder to destination folder using xcopy command
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ResponseMessage> XCopy(XCopyRequest message, ServerCallContext context)
        {
            try
            {
                var arg = "\"" + message.SourceFolderPath + "\"" + " " + "\"" + message.TargetFolderPath + "\"" + @" /e /y /I";
                StartIndividualProcess("xcopy", arg);
                return ReturnResult(true, $"Copied files from '{message.SourceFolderPath}' to '{message.TargetFolderPath}'", "");
            }
            catch (Exception ex)
            {
                return ReturnResult(false, "Error", ex.Message);
            }
        }

        /// <summary>
        /// Copy specified version folder to application root dircetory (specified in message) and start another updated instance
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ResponseMessage> SelfUpdate(SelfUpdateRequest request, ServerCallContext context)
        {
            try
            {
                var arg = "\"" + request.SourceFolderPath + "\"" + " " + "\"" + request.TargetFolderPath + "\"" + @" /e /y /I";
                StartIndividualProcess("xcopy", arg);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                throw;
            }
            try
            {
                StartIndividualProcess(Path.Combine(request.TargetFolderPath, request.TargetFileToRun), "", false);
                //TO-DO - Environment.Exit(0);
                return ReturnResult(true, $"Copied files from '{request.SourceFolderPath}' to '{request.TargetFolderPath}'", "").Result;
            }
            catch (Exception ex)
            {
                return ReturnResult(false, "Error", ex.Message).Result ;
            }
        }

        /// <summary>
        /// One method to return result
        /// </summary>
        /// <param name="success">true if succeed</param>
        /// <param name="message">response message</param>
        /// <param name="error">error if any</param>
        /// <returns></returns>
        private async Task<ResponseMessage> ReturnResult(bool success, string message, string error)
        {
            return (new ResponseMessage
            {
                Success = success,
                Message = message,
                Error = error
            });
        }

        /// <summary>
        /// Starts a process with separate window
        /// </summary>
        /// <param name="FileName">Name of the file to execute</param>
        /// <param name="Arguments">Required list of arguments</param>
        /// <param name="WaitForExit">Whether to wait till exit</param>
        private void StartIndividualProcess(string FileName, string Arguments, bool WaitForExit = true)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            //Give the name as Xcopy
            startInfo.FileName = FileName;
            //make the window Hidden
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            //Send the Source and destination as Arguments to the process
            startInfo.Arguments = Arguments;
            startInfo.WorkingDirectory = Path.GetDirectoryName(FileName);
            //startInfo.RedirectStandardError = true;
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    if (WaitForExit)
                    {
                        exeProcess.WaitForExit();
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                throw;
            }
        }
    }
}
