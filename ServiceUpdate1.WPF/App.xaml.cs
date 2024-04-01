using ServiceUpdate1.WPF.Services;
using ServiceUpdate1.WPF.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;
using System.Windows;

namespace ServiceUpdate1.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            HubConnection connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/colorchat")
                .Build();

            ColorChatViewModel chatViewModel = ColorChatViewModel.CreatedConnectedViewModel(new SignalRChatService(connection));

            MainWindow window = new MainWindow
            {
                DataContext = new MainViewModel(chatViewModel)
            };

            window.Show();
        }
    }
}
