using ServiceUpdate1.WPFServiceUpdater.Lib;
//using GalaSoft.MvvmLight.CommandWpf;
using ServiceUpdate1.WPFServiceUpdater.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using ServiceUpdate1.GrpcClient;
using System.Net;
using System.Drawing;
using System.Windows.Media;
using System.IO;

namespace ServiceUpdate1.WPFServiceUpdater.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //public ObservableCollection<ServiceModel> Services { get; set; }
        public ObservableCollection<Machine> Machines { get; set; }
        private Machine? _selectedMachine = null;
        private GRPCClientHelper? _clientHelper = null;
        public GRPCClientHelper ClientHelper
        {
            get
            {
                //if (_clientHelper == null)
                //    _clientHelper = new GRPCClientHelper(IPAddress.Parse(_selectedMachine.MachineIPAddress),
                //        _selectedMachine.Port, _selectedMachine.InstalledFilePath);

                return new GRPCClientHelper(IPAddress.Parse(_selectedMachine.MachineIPAddress),
                        _selectedMachine.Port,
                        _selectedMachine.InstalledFilePath,
                        _selectedMachine.LatestVersion,
                        _selectedMachine.TargetFolderPath);
            }
        }

        public MainViewModel()
        {
            //Services = new ObservableCollection<ServiceModel>();
            var machineCollection = new MachineCollection();
            machineCollection.LoadMachines();
            Machines = machineCollection.Machines;

            // Initialize Services collection and populate with dummy data
            //    Services = new ObservableCollection<ServiceModel>
            //{
            //    new ServiceModel { SerialNumber = 1, MachineName="HOST1", MachineIPAddress= new System.Net.IPAddress( [127,0,0,1]), ServiceName = "Service A", InstalledVersion = "1.0", LatestVersion = "1.2" },
            //    new ServiceModel { SerialNumber = 2, MachineName="HOST2", MachineIPAddress= new System.Net.IPAddress( [127,0,0,1]), ServiceName = "Service B", InstalledVersion = "1.0", LatestVersion = "1.2" },
            //    new ServiceModel { SerialNumber = 3, MachineName="HOST3", MachineIPAddress= new System.Net.IPAddress( [127,0,0,1]), ServiceName = "Service C", InstalledVersion = "2.1", LatestVersion = "2.3" },
            //    // Add more dummy data as needed
            //};
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private System.Drawing.Color _bGColor;
        public System.Drawing.Color BGColor
        {
            get { return _bGColor; }
            set { _bGColor = value; OnPropertyChanged(nameof(BGColor)); }
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

        private RelayCommand _getInstalledVersionCommand;
        public ICommand GetInstalledVersionCommand
        {
            get
            {
                if (_getInstalledVersionCommand == null)
                {
                    _getInstalledVersionCommand = new RelayCommand(param => GetInstalledVersionAsync(param));
                }
                return _getInstalledVersionCommand;
            }
        }
        private RelayCommand _uploadFileCommand;
        public ICommand UploadFileCommand
        {
            get
            {
                if (_uploadFileCommand == null)
                {
                    _uploadFileCommand = new RelayCommand(param => UploadFile(param));
                }
                return _uploadFileCommand;
            }
        }

        private async void GetInstalledVersionAsync(object param)
        {
            _selectedMachine = (Machine)param;
            //byte[] bytes = Encoding.ASCII.GetBytes(a.MachineIPAddress);
            //Console.WriteLine(a.MachineName );
            //var address = System.Net.IPAddress.Parse(_selectedMachine.MachineIPAddress);
            //GRPCClientHelper clientHelper = new GRPCClientHelper(System.Net.IPAddress.Parse(_selectedMachine.MachineIPAddress),
            //    _selectedMachine.Port , _selectedMachine.InstalledFilePath );
            var result = await ClientHelper.GetFileInstalledVersion();
            if (result == "UNSUCCESS")
                MessageBox.Show("UNSUCCESS", "Information - Version Info Reply");
            else
                _selectedMachine.InstalledVersion = result;
        }

        private async void UpdateService(object param)
        {
            BGColor = System.Drawing.Color.Gray;
            _selectedMachine = (Machine)param;
            var result = await ClientHelper.SendUpdateRequest();
            if (result != GRPCClientHelperResponse.SUCCESS)
                MessageBox.Show("UNSUCCESS", "Information - Send Update Reply");
            else
            {
                var versionResult = await ClientHelper.GetFileInstalledVersion();
                if (versionResult == "UNSUCCESS")
                    MessageBox.Show("UNSUCCESS", "Information - Version Info Reply");
                else
                    MessageBox.Show("SUCCESS", "Information - Updated file.");
                _selectedMachine.InstalledVersion = versionResult;
            }
            BGColor = System.Drawing.Color.Green;
        }

        private async void UploadFile(object param)
        {
            _selectedMachine = (Machine)param;
            var result = await ClientHelper.UploadFile();
            if (!result)
                MessageBox.Show("UNSUCCESS", "Information -  File upload failed.");
            else
                MessageBox.Show("SUCCESS", "Information - File uploaded successfully.");
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }

}
