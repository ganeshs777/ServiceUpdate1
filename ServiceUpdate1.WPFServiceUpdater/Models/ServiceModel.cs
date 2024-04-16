using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceUpdate1.WPFServiceUpdater.Models
{
    public class ServiceModel : INotifyPropertyChanged
    {
        private int _serialNumber;
        public int SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; OnPropertyChanged(nameof(SerialNumber)); }
        }
        
        private string _machineName;
        public string MachineName
        {
            get { return _machineName; }
            set { _machineName = value; OnPropertyChanged(nameof(MachineName)); }
        }

        System.Net.IPAddress _machineIPAddress;
        public System.Net.IPAddress MachineIPAddress
        {
            get { return _machineIPAddress; }
            set { _machineIPAddress = value; OnPropertyChanged(nameof(MachineIPAddress)); }
        }

        private string _serviceName;
        public string ServiceName
        {
            get { return _serviceName; }
            set { _serviceName = value; OnPropertyChanged(nameof(ServiceName)); }
        }

        private string _installedVersion;
        public string InstalledVersion
        {
            get { return _installedVersion; }
            set { _installedVersion = value; OnPropertyChanged(nameof(InstalledVersion)); }
        }

        private string _latestVersion;
        public string LatestVersion
        {
            get { return _latestVersion; }
            set { _latestVersion = value; OnPropertyChanged(nameof(LatestVersion)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
