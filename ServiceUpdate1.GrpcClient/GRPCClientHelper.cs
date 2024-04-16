using Google.Protobuf;
using Grpc.Net.Client;
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
        IPAddress _iPAddress = new([127, 0, 0, 1]);
        int _port = 5048;
        string _filePath;
        private bool _validInputs;
        public GRPCClientHelper(IPAddress IPAddress, int Port, string FilePath)
        {
            _iPAddress = IPAddress;
            _port = Port;
            _filePath = FilePath;
            _validInputs = ValidateInputs();
        }

        public async Task<GRPCClientHelperResponse> SendUpdateRequest()
        {
            if (!_validInputs)
                return GRPCClientHelperResponse.INVALID_REQUEST_INPUTS;
            if (!_iPAddress.IsAccessible())
                return GRPCClientHelperResponse.HOST_NOT_ACCESSIBLE;
            using (var channel = GrpcChannel.ForAddress($"http://{_iPAddress}:{_port}"))
            {
                var client = new DeployUpdatesServiceClient(channel);
                //var filePath = _filePath;// AppContext.BaseDirectory + "ServiceUpdate1.GrpcClient.exe";// "path/to/your/file.txt";
                byte[] fileBytes = File.ReadAllBytes(_filePath);
                var reply = await client.SendUpdatesAsync(new FileMessage
                {
                    Filename = Path.GetFileNameWithoutExtension(_filePath),
                    ContentPath = _filePath,
                    Content = ByteString.CopyFrom(fileBytes)
                });
                //Console.WriteLine("Service update : " + reply.Message);
                if (reply.Message == "SUCCESS")
                    return GRPCClientHelperResponse.SUCCESS;
            }
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
                var client = new DeployUpdatesServiceClient(channel);
                //var filePath = _filePath;// AppContext.BaseDirectory + "ServiceUpdate1.GrpcClient.exe";// "path/to/your/file.txt";
                var reply = await client.InstallUpdatesAsync(new Empty());
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
                using (var channel = GrpcChannel.ForAddress($"http://{_iPAddress}:{_port}"))
                {
                    var client = new DeployUpdatesServiceClient(channel);
                    //var filePath = _filePath;// AppContext.BaseDirectory + "ServiceUpdate1.GrpcClient.exe";// "path/to/your/file.txt";
                    byte[] fileBytes = File.ReadAllBytes(_filePath);
                    var reply = await client.GetLatestVersionAsync(new Empty());
                    //Console.WriteLine("Service update : " + reply.Message);
                    return reply.Version;
                }
            }
            catch (Exception)
            {

            }
            return "UNSUCCESS";
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
