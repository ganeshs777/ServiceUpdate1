using Microsoft.Extensions.Options;
using ServiceUpdate1.GrpcServer.Services;
using System.Data;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

//var config = ReadConfigFromXml("config.xml");
//Console.WriteLine($"Folder Path: {config}");
string serverURL = ReadConfigFromXml("config.xml"); 

builder.WebHost.UseUrls(serverURL);
//builder.WebHost.UseUrls("https://192.168.1.4:5047","http://192.168.1.4:5048");

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.MapGrpcService<UpdateServiceImp>();
app.MapGrpcService<DeployUpdatesService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

static string ReadConfigFromXml(string filePath)
{
    try
    {
        var doc = XDocument.Load(filePath);
        var configurationNode = doc.Element("configuration");
        if (configurationNode != null)
        {
            var serverURLNode = configurationNode.Element("ServerURL");
            if (serverURLNode != null)
            {
                return serverURLNode.Value;
            }
        }
    }
    catch (Exception)
    {
        throw;
        //Console.WriteLine("Configuration error.");
    }
    return "";
}
