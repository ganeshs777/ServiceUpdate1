
using Grpc.Core;
using Microsoft.Extensions.Logging;
using ServiceUpdate1.GrpcServer;

namespace ServiceUpdate1.ServiceHost
{
    public class TimerService1 : BackgroundService
    {
        private readonly ILogger<TimerService> _logger;
        Server _server;

        public TimerService1(ILogger<TimerService> logger)
        {
            _logger = logger;
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            
            //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous Start operation.</returns>
        //public override Task StartAsync(CancellationToken cancellationToken)
        //{
        //    var result = base.StartAsync(cancellationToken);

        //    _server = new Server
        //    {
        //        //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //        Services = { ServiceUpdate1.GrpcServer.DeployUpdatesService.BindService(new ServiceUpdate1.GrpcServer.Services.DeployUpdatesService(_logger, @"C:\Update Installer")) },
        //        //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //        //Ports = { new ServerPort("127.0.0.1", 5048, ServerCredentials.Insecure) }
        //        Ports = { new ServerPort("10.5.92.170", 5048, ServerCredentials.Insecure) }
        //        //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //    };
        //    _logger.LogInformation("Server Starting : {time}", DateTimeOffset.Now);
        //    _server.Start();
        //    _logger.LogInformation("Server Started : {time}", DateTimeOffset.Now);

        //    // Otherwise it's running
        //    return Task.CompletedTask;
        //}

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ////Do nothing
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    if (_logger.IsEnabled(LogLevel.Information))
            //    {
            //        //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    }
            //    await Task.Delay(1000, stoppingToken);
            //}
            await Task.CompletedTask;
        }

        //protected override async Task
        //

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous Stop operation.</returns>
        public override  async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Server Stopped : {time}", DateTimeOffset.Now);
            await base.StopAsync(cancellationToken);
            return;

        }
    }

    public sealed class TimerService(ILogger<TimerService> logger) : IHostedService, IAsyncDisposable
    {
        private readonly Task _completedTask = Task.CompletedTask;
        private int _executionCount = 0;
        private Timer? _timer;
        Server? _server;
        public Task StartAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("{Service} is running.", nameof(TimerService));
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            //_server = new Server
            //{
            //    Services = { ServiceUpdate1.GrpcServer.DeployUpdatesService.BindService(
            //        new ServiceUpdate1.GrpcServer.Services.DeployUpdatesService(logger, @"C:\Update Installer")) },
            //    //Ports = { new ServerPort("127.0.0.1", 5048, ServerCredentials.Insecure) }
            //    Ports = { new ServerPort("10.5.92.170", 5048, ServerCredentials.Insecure) }
            //};
            //logger.LogInformation("Server Starting : {time}", DateTimeOffset.Now);
            //_server.Start();
            logger.LogInformation("Server Started : {time}", DateTimeOffset.Now);
            return _completedTask;
        }

        private void DoWork(object? state)
        {
            int count = Interlocked.Increment(ref _executionCount);

            logger.LogInformation(
                "{Service} is working, execution count: {Count:#,0}",
                nameof(TimerService),
                count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation(
                "{Service} is stopping.", nameof(TimerService));

            _timer?.Change(Timeout.Infinite, 0);
            logger.LogInformation("Server Starting : {time}", DateTimeOffset.Now);
            _server?.ShutdownAsync();
            logger.LogInformation("Server Started : {time}", DateTimeOffset.Now);
            return _completedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (_timer is IAsyncDisposable timer)
            {
                await timer.DisposeAsync();
            }

            _timer = null;
        }
    }
}
