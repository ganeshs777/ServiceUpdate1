// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ServiceUpdate1;
public class FileSystemMonitor
{
    private readonly string _path;
    private FileSystemWatcher? _watcher;

    public FileSystemMonitor(string path)
    {
        _path = path;
    }

    public void StartMonitoring()
    {
        _watcher = new FileSystemWatcher(_path);

        _watcher.Created += OnCreated;
        _watcher.Deleted += OnDeleted;
        _watcher.Renamed += OnRenamed;

        _watcher.IncludeSubdirectories = true; // Optional: monitor subdirectories as well
        _watcher.EnableRaisingEvents = true;
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"File created: {e.FullPath}");
        // to - do
        // verify setup version and update service.
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"File deleted: {e.FullPath}");
        // to - do
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        Console.WriteLine($"File renamed: {e.OldFullPath} -> {e.FullPath}");
        // to - do
    }

    public void StopMonitoring()
    {
        if (_watcher == null)
            return;
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
    }

    public static void Main(string[] args)
    {
        var config = ReadConfigFromXml("config.xml");
        Console.WriteLine($"Folder Path: {config}");
        string pathToMonitor = ReadConfigFromXml("config.xml"); //@"C:\Service update setup\"; 
        if (Directory.Exists(pathToMonitor))
        {
            FileSystemMonitor monitor = new (pathToMonitor);
            monitor.StartMonitoring();
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
            monitor.StopMonitoring();
        }
        else
        {
            Console.WriteLine("Path does not exists. Press any key to exit...");
            Console.ReadKey();
        }

    }
    static string ReadConfigFromXml(string filePath)
    {
        try
        {
            var doc = XDocument.Load(filePath);
            var configurationNode = doc.Element("configuration");
            if (configurationNode != null)
            {
                var folderPathNode = configurationNode.Element("folderPath");
                if (folderPathNode != null)
                {
                    return folderPathNode.Value;
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Configuration error.");
        }
        return "";
    }
}
