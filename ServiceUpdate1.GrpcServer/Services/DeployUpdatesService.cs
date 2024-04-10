using Google.Protobuf;
using Grpc.Core;

namespace ServiceUpdate1.GrpcServer.Services
{
    public class DeployUpdatesService : GrpcServer.DeployUpdatesService.DeployUpdatesServiceBase
    {
        public override Task<ResponseMessage> SendUpdates(FileMessage message, ServerCallContext context)
        {
            string outputFilePath = AppContext.BaseDirectory + "ServiceUpdate1.GrpcClient1.exe";
            ByteString byteString = message.Content;
            using (FileStream outputStream = File.OpenWrite(outputFilePath))
            {
                byteString.WriteTo(outputStream);
            }
            return Task.FromResult(new ResponseMessage { Message = "SUCCESS" });
        }

    }
}
