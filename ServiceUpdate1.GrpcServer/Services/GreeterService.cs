using Grpc.Core;
using Microsoft.AspNetCore.Hosting.Server;
using ServiceUpdate1.GrpcServer;
using ServiceUpdate1.ObserverSubscriber.Provider;
using ServiceUpdate1.ObserverSubscriber.Subscriber;
using System;
using System.Threading.Tasks;

namespace ServiceUpdate1.GrpcServer.Services
{

    public class UpdateServiceImp : GrpcServer.UpdateService.UpdateServiceBase
    {
        private readonly string _currentVersion = "1.1.0"; // Example: Simulate a newer version
        private static readonly NotificationProvider _NotificationProvider = new("BH");

        public override Task<VersionInfo> GetLatestVersion(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new VersionInfo { Version = _currentVersion });
        }

        // Implement DownloadUpdate method logic here if used (commented out in proto)
        // public override Task<UpdatePackage> DownloadUpdate(VersionInfo request, ServerCallContext context)
        // {
        //     // ... (logic to provide update package)
        // }

        public override Task<SubscribeResponse> Subscribe(SubscribeRequest request, ServerCallContext context)
        {
            try
            {
                var observer = new NotificationSubscriber(request.ClientName);
                observer.Subscribe(_NotificationProvider);
                return Task.FromResult(new SubscribeResponse { Subscribed = true });
            }
            catch (Exception)
            {
                //return Task.FromResult(new SubscribeResponse { Subscribed = false });
                //throw;
            }
            return Task.FromResult(new SubscribeResponse { Subscribed = false });
        }
    }

    public class UpdateServer
    {
        public static void Main(string[] args)
        {
            var server = new Server
            {
                Services = { GrpcServer.UpdateService.BindService(new UpdateServiceImp()) },
                Ports = { new ServerPort("localhost", 50051, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Update server listening on port 50051");
            Console.ReadLine();
        }
    }

}
