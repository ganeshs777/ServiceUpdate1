
using ServiceUpdate1.ServiceHost;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<TimerService>();

var host = builder.Build();
host.Run();
