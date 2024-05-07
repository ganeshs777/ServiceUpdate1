using Google.Protobuf;
using Grpc.Net.Client;
using ServiceUpdate1.GrpcServer;
using System.Net;
using static ServiceUpdate1.GrpcServer.DeployUpdatesService;

namespace ServiceUpdate1.GrpcClient
{
    public class GRPCClientHelper
    {
        private readonly IPAddress _iPAddress;
        int _port = 5048;
        string _filePath;
        private readonly GrpcChannel _channel;
        private readonly DeployUpdatesService.DeployUpdatesServiceClient _client;
        private const int ChunkSize = 1024 * 32; // 32 KB
        private string _sourceFolderPath;
        private string _targetFolderPath;

        public GRPCClientHelper(IPAddress IPAddress, int Port, string FilePath, string SourceFolderPath, string TargetFolderPath)
        {
            _iPAddress = IPAddress;
            _port = Port;
            _filePath = FilePath;
            _sourceFolderPath = SourceFolderPath;
            _targetFolderPath = TargetFolderPath;
            GrpcChannelOptions channelOptions = new GrpcChannelOptions();
            channelOptions.MaxSendMessageSize = int.MaxValue;
            channelOptions.MaxReceiveMessageSize = int.MaxValue;
            _channel = GrpcChannel.ForAddress($"http://{_iPAddress}:{_port}", channelOptions);
            _client = new DeployUpdatesServiceClient(_channel);
        }

        public async Task<ResponseMessage> SendUpdates()
        {
            byte[] fileBytes = File.ReadAllBytes(_filePath);
            return await _client.SendUpdatesAsync(new FileMessage
            {
                Filename = Path.GetFileName(_filePath),
                ContentPath = _filePath,
                Content = ByteString.CopyFrom(fileBytes)
            });
        }

        public async Task<ResponseMessage> InstallUpdates()
        {
            return await _client.InstallUpdatesAsync(new Empty());
        }

        public async Task<ResponseMessage> GetFileInstalledVersion()
        {
            return await _client.GetLatestVersionAsync(new Empty());
        }

        public async Task<ResponseMessage> UploadFile()
        {
            Console.WriteLine("Starting call");
            var call = _client.UploadFile();

            Console.WriteLine("Sending file metadata");
            await call.RequestStream.WriteAsync(new UploadFileRequest
            {
                Metadata = new UploadFileMetadata
                {
                    FileName = Path.GetFileName(_filePath),
                    SourceFolderPath = _sourceFolderPath,
                    TargetFolderPath = _targetFolderPath
                }
            });

            var buffer = new byte[ChunkSize];
            await using var readStream = File.OpenRead(_filePath);

            while (true)
            {
                var count = await readStream.ReadAsync(buffer);
                if (count == 0)
                {
                    break;
                }

                Console.WriteLine("Sending file data chunk of length " + count);
                await call.RequestStream.WriteAsync(new UploadFileRequest
                {
                    Data = UnsafeByteOperations.UnsafeWrap(buffer.AsMemory(0, count))
                });
            }

            Console.WriteLine("Complete request");
            await call.RequestStream.CompleteAsync();

            var response = await call;
            Console.WriteLine("Shutting down");
            return response;
        }

        public async Task<ResponseMessage> XCopy()
        {
            return await _client.XCopyAsync(new XCopyRequest
            {
                SourceFolderPath = _sourceFolderPath,
                TargetFolderPath = _targetFolderPath,
            });
        }

        public async Task<ResponseMessage> SelfUpdate()
        {
            return await _client.SelfUpdateAsync(new SelfUpdateRequest
            {
                SourceFolderPath = _sourceFolderPath,
                TargetFolderPath = _targetFolderPath,
                TargetFileToRun = Path.GetFileName(_filePath)
            });
        }

        public ResponseMessage DummyResult()
        {
            return (new ResponseMessage());
        }
    }
}
