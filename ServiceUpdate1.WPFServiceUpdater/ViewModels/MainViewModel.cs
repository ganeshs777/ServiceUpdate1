using ServiceUpdate1.WPFServiceUpdater.Lib;
//using GalaSoft.MvvmLight.CommandWpf;
using ServiceUpdate1.WPFServiceUpdater.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ServiceUpdate1.WPFServiceUpdater.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ServiceModel> Services { get; set; }

        public MainViewModel()
        {
            // Initialize Services collection and populate with dummy data
            Services = new ObservableCollection<ServiceModel>
        {
            new ServiceModel { SerialNumber = 1, ServiceName = "Service A", InstalledVersion = "1.0", RemoteVersion = "1.2" },
            new ServiceModel { SerialNumber = 2, ServiceName = "Service B", InstalledVersion = "2.1", RemoteVersion = "2.3" },
            // Add more dummy data as needed
        };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Define command for the Update button
        private RelayCommand _updateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                if (_updateCommand == null)
                {
                    _updateCommand = new RelayCommand(param => UpdateService(param));
                }
                return _updateCommand;
            }
        }

        private void UpdateService(object param)
        {
            // Implement update logic here
        }
    }

}
