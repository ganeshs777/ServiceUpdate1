using Google.Protobuf;
using Grpc.Net.Client;
using ServiceUpdate1.GrpcServer;
using static ServiceUpdate1.GrpcServer.DeployUpdatesService;


using var channel = GrpcChannel.ForAddress("http://localhost:5048");
var client = new DeployUpdatesServiceClient(channel);
var filePath = AppContext.BaseDirectory + "ServiceUpdate1.GrpcClient.exe";// "path/to/your/file.txt";
byte[] fileBytes = File.ReadAllBytes(filePath);

var reply = await client.SendUpdatesAsync(new FileMessage
{
    Filename = "Test.exe",
    TargetFolderPath = "URL",
    Content = ByteString.CopyFrom(fileBytes)
}) ;
Console.WriteLine("Service update : " + reply.Message );

//// The port number must match the port of the gRPC server.
//using var channel = GrpcChannel.ForAddress("http://localhost:5048");
//var client = new UpdateServiceClient(channel);
//var reply = await client.GetLatestVersionAsync(new Empty());
//Console.WriteLine("Greeting: " + reply.Version);
//var reply1 = await client.SubscribeAsync(
//    new SubscribeRequest
//    {
//        ClientName = "Client1",
//        ClientUrl = "Url1",
//        ServiceName = "Service1"
//    }) ;
//Console.WriteLine("Greeting: " + reply1.Subscribed );
Console.WriteLine("Press any key to exit...");
Console.ReadKey();