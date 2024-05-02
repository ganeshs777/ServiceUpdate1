using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using ServiceUpdate1.GrpcServer;
using System.Net;
using System.Runtime.InteropServices;
using static ServiceUpdate1.GrpcServer.DeployUpdatesService;

namespace ServiceUpdate1.GrpcClient
{
    public enum GRPCClientHelperResponse
    {
        SUCCESS,
        FAILED,
        INTERNAL_ERROR,
        VALID_INPUTS,
        INVALID_IPADDRESS,
        INVALID_PORT,
        FILE_NOT_FOUND,
        INVALID_REQUEST_INPUTS,
        HOST_NOT_ACCESSIBLE,
        VERSION_INFO
    }
    public class GRPCClientHelper
    {
        private readonly IPAddress _iPAddress ;
        int _port = 5048;
        string _filePath;
        private bool _validInputs;
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
            _validInputs = ValidateInputs();
            GrpcChannelOptions channelOptions = new GrpcChannelOptions();
            channelOptions.MaxSendMessageSize = int.MaxValue;
            channelOptions.MaxReceiveMessageSize = int.MaxValue;
            _channel = GrpcChannel.ForAddress($"http://{_iPAddress}:{_port}", channelOptions);
            _client = new DeployUpdatesServiceClient(_channel);

        }

        public async Task<GRPCClientHelperResponse> SendUpdateRequest()
        {
            //        .insecure_channel(
            //'localhost:50051',
            //options = [
            //    ('grpc.max_send_message_length', MAX_MESSAGE_LENGTH),
            //    ('grpc.max_receive_message_length', MAX_MESSAGE_LENGTH),
            //],
            //)
            if (!_validInputs)
                return GRPCClientHelperResponse.INVALID_REQUEST_INPUTS;
            if (!_iPAddress.IsAccessible())
                return GRPCClientHelperResponse.HOST_NOT_ACCESSIBLE;
            //    var channelOptions = new List<ChannelOption>()
            //{
            //     new ChannelOption(ChannelOptions.MaxReceiveMessageLength, int.MaxValue),
            //     new ChannelOption(ChannelOptions.MaxSendMessageLength, int.MaxValue),
            //};
            //GrpcChannelOptions channelOptions = new GrpcChannelOptions();
            //channelOptions.MaxSendMessageSize = int.MaxValue;

            //var filePath = _filePath;// AppContext.BaseDirectory + "ServiceUpdate1.GrpcClient.exe";// "path/to/your/file.txt";
            byte[] fileBytes = File.ReadAllBytes(_filePath);
            var reply = await _client.SendUpdatesAsync(new FileMessage
            {
                Filename = Path.GetFileName(_filePath),
                ContentPath = _filePath,
                Content = ByteString.CopyFrom(fileBytes)
            });
            //Console.WriteLine("Service update : " + reply.Message);
            if (reply.Message == "SUCCESS")
                return GRPCClientHelperResponse.SUCCESS;
            return GRPCClientHelperResponse.FAILED;
        }

        public async Task<GRPCClientHelperResponse> InstallUpdateRequest()
        {
            if (!_validInputs)
                return GRPCClientHelperResponse.INVALID_REQUEST_INPUTS;
            if (!_iPAddress.IsAccessible())
                return GRPCClientHelperResponse.HOST_NOT_ACCESSIBLE;
            using (var channel = GrpcChannel.ForAddress($"http://{_iPAddress}:{_port}"))
            {
                //var client = new DeployUpdatesServiceClient(channel);
                //var filePath = _filePath;// AppContext.BaseDirectory + "ServiceUpdate1.GrpcClient.exe";// "path/to/your/file.txt";
                var reply = await _client.InstallUpdatesAsync(new Empty());
                //Console.WriteLine("Service update : " + reply.Message);
                if (reply.Message == "SUCCESS")
                    return GRPCClientHelperResponse.SUCCESS;
            }
            return GRPCClientHelperResponse.FAILED;
        }

        public async Task<string> GetFileInstalledVersion()
        {
            if (!_validInputs)
                return "Invalid request inputs";
            if (!_iPAddress.IsAccessible())
                return "Host not accessible";
            try
            {
                var client = new DeployUpdatesServiceClient(_channel);
                //var filePath = _filePath;// AppContext.BaseDirectory + "ServiceUpdate1.GrpcClient.exe";// "path/to/your/file.txt";
                byte[] fileBytes = File.ReadAllBytes(_filePath);
                var reply = await client.GetLatestVersionAsync(new Empty());
                //Console.WriteLine("Service update : " + reply.Message);
                return reply.Version;
            }
            catch (Exception)
            {

            }
            return "UNSUCCESS";
        }

        public async Task<bool> UploadFile()
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
            }) ;

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
            Console.WriteLine("Upload id: " + response.Id);

            Console.WriteLine("Shutting down");
            return true;
        }

        private bool ValidateInputs()
        {
            if (_filePath == null)
                return false;
            if (!File.Exists(_filePath))
                return false;
            return true;
        }
    }
}
