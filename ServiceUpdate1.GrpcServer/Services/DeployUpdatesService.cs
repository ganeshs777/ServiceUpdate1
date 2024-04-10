using Google.Protobuf;
using Grpc.Core;
using System.Diagnostics.Metrics;

namespace ServiceUpdate1.GrpcServer.Services
{
    public class DeployUpdatesService : GrpcServer.DeployUpdatesService.DeployUpdatesServiceBase
    {
        private static int counter = 0;
        public override Task<ResponseMessage> SendUpdates(FileMessage message, ServerCallContext context)
        {
            counter++;
            string outputFilePath = AppContext.BaseDirectory + $"ServiceUpdate1.GrpcClient{counter}.exe";
            ByteString byteString = message.Content;
            using (FileStream outputStream = File.OpenWrite(outputFilePath))
            {
                byteString.WriteTo(outputStream);
            }
            return Task.FromResult(new ResponseMessage { Message = "SUCCESS" + counter });
        }

    }
}
