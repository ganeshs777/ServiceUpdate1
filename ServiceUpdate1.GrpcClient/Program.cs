using System.Threading.Tasks;
using Grpc.Net.Client;
using ServiceUpdate1.GrpcClient;
using static ServiceUpdate1.GrpcClient.UpdateService;


// The port number must match the port of the gRPC server.
using var channel = GrpcChannel.ForAddress("http://localhost:5048");
var client = new UpdateServiceClient(channel);
var reply = await client.GetLatestVersionAsync(new Empty());
Console.WriteLine("Greeting: " + reply.Version);
var reply1 = await client.SubscribeAsync(
    new SubscribeRequest
    {
        ClientName = "Client1",
        ClientUrl = "Url1",
        ServiceName = "Service1"
    }) ;
Console.WriteLine("Greeting: " + reply1.Subscribed );
Console.WriteLine("Press any key to exit...");
Console.ReadKey();