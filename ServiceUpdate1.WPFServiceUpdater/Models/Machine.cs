using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServiceUpdate1.WPFServiceUpdater.Models
{
    [Serializable()]
    public class Machine : INotifyPropertyChanged
    {
        private string _installedVersion;

        [XmlAttribute(AttributeName = "id")]
        public int SerialNumber { get; set; }

        [XmlElement(ElementName = "machinename")]
        public string MachineName { get; set; }

        [XmlElement(ElementName = "machineipaddress")]
        public string MachineIPAddress { get; set; }

        [XmlElement(ElementName = "port")]
        public int Port { get; set; }

        [XmlElement(ElementName = "servicename")]
        public string ServiceName { get; set; }
        
        [XmlElement(ElementName = "installedversion")]
        public string InstalledVersion
        {
            get { return _installedVersion; }
            set { _installedVersion = value; OnPropertyChanged(nameof(InstalledVersion)); }
        }

        [XmlElement(ElementName = "latestversion")]
        public string LatestVersion { get; set; }

        [XmlElement(ElementName = "installedfilepath")]
        public string InstalledFilePath { get; set; }

        [XmlElement(ElementName = "TargetFolderPath")]
        public string TargetFolderPath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
