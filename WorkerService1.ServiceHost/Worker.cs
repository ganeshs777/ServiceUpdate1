
using Grpc.Core;
using ServiceUpdate1.GrpcServer;

namespace ServiceUpdate1.ServiceHost
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            var server = new Server
            {
                Services = { ServiceUpdate1.GrpcServer.DeployUpdatesService.BindService(new ServiceUpdate1.GrpcServer.Services.DeployUpdatesService(logger, @"C:\Update Installer")) },
                Ports = { new ServerPort("127.0.0.1", 5048, ServerCredentials.Insecure) }
                //Ports = { new ServerPort("10.5.92.167", 5000, ServerCredentials.Insecure) }
            };
            server.Start();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        //protected override async Task       
    }
}
