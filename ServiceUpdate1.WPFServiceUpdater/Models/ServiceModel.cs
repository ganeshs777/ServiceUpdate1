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

        private string _remoteVersion;
        public string RemoteVersion
        {
            get { return _remoteVersion; }
            set { _remoteVersion = value; OnPropertyChanged(nameof(RemoteVersion)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
