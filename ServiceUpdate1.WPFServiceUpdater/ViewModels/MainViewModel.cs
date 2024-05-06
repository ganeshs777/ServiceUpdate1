using ServiceUpdate1.GrpcClient;
using ServiceUpdate1.WPFServiceUpdater.Lib;
//using GalaSoft.MvvmLight.CommandWpf;
using ServiceUpdate1.WPFServiceUpdater.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows.Controls;
using System.Windows.Input;

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

        private string _gRPCMethodResponse;
        public string gRPCMethodResponse
        {
            get { return _gRPCMethodResponse; }
            set { _gRPCMethodResponse = value; OnPropertyChanged(nameof(gRPCMethodResponse)); }
        }

        private string _methodName;
        public string MethodName
        {
            get { return _methodName; }
            set { _methodName = value; OnPropertyChanged(nameof(MethodName)); }
        }

        private string _methodParameters;
        public string MethodParameters
        {
            get { return _methodParameters; }
            set { _methodParameters = value; OnPropertyChanged(nameof(MethodParameters)); }
        }

        private string _methodError;
        public string MethodError
        {
            get { return _methodError; }
            set { _methodError = value; OnPropertyChanged(nameof(MethodError)); }
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

        private RelayCommand _xCopyCommand;
        public ICommand XCopyCommand
        {
            get
            {
                if (_xCopyCommand == null)
                {
                    _xCopyCommand = new RelayCommand(param => XCopyFolder(param));
                }
                return _xCopyCommand;
            }
        }

        private RelayCommand _SelfUpdateCommand;
        public ICommand SelfUpdateCommand
        {
            get
            {
                if (_SelfUpdateCommand == null)
                {
                    _SelfUpdateCommand = new RelayCommand(param => SelfUpdate(param));
                }
                return _SelfUpdateCommand;
            }
        }

        private bool ConvertDataGridrowToMachineObject(object param)
        {
            if (param is not Machine)
                return false;
            _selectedMachine = (Machine)param;
            return true;
        }

        private async void GetInstalledVersionAsync(object param)
        {
            if (ConvertDataGridrowToMachineObject(param))
            {
                _selectedMachine = (Machine)param;
                MethodName = "GetInstalledVersionAsync";
                MethodParameters = "Parameters : IP Address : " + _selectedMachine.MachineIPAddress +
                    "\n File Path : " + _selectedMachine.InstalledFilePath;
                //byte[] bytes = Encoding.ASCII.GetBytes(a.MachineIPAddress);
                //Console.WriteLine(a.MachineName );
                //var address = System.Net.IPAddress.Parse(_selectedMachine.MachineIPAddress);
                //GRPCClientHelper clientHelper = new GRPCClientHelper(System.Net.IPAddress.Parse(_selectedMachine.MachineIPAddress),
                //    _selectedMachine.Port , _selectedMachine.InstalledFilePath );
                var result = await ClientHelper.GetFileInstalledVersion();
                gRPCMethodResponse = result;
                MethodError = "";
                if (result == "UNSUCCESS")
                    MethodError = result;
                else
                    _selectedMachine.InstalledVersion = result; 
            }
        }

        private async void UpdateService(object param)
        {
            if(ConvertDataGridrowToMachineObject(param))
            {
                MethodName = "UpdateService";
                MethodParameters = "Parameters : IP Address : " + _selectedMachine.MachineIPAddress +
                    "\n File Path : " + _selectedMachine.InstalledFilePath;
                var result = await ClientHelper.SendUpdateRequest();
                gRPCMethodResponse = result.ToString();
                MethodError = "";
                if (result != GRPCClientHelperResponse.SUCCESS)
                    MethodError = "UNSUCCESS";
                else
                {
                    var versionResult = await ClientHelper.GetFileInstalledVersion();
                    if (versionResult == "UNSUCCESS")
                        MethodError = "UNSUCCESS";
                    else
                        _selectedMachine.InstalledVersion = versionResult;
                }
            }
        }

        private async void UploadFile(object param)
        {
            if (ConvertDataGridrowToMachineObject(param))
            {
                _selectedMachine = (Machine)param;
                MethodName = "UploadFile";
                MethodParameters = "Parameters : IP Address : " + _selectedMachine.MachineIPAddress +
                    "\n File Path : " + _selectedMachine.InstalledFilePath;
                var result = await ClientHelper.UploadFile();
                gRPCMethodResponse = result.ToString();
                MethodError = "";
                if (!result)
                    MethodError = "UNSUCCESS";
            }
        }

        private async void XCopyFolder(object param)
        {
            if (ConvertDataGridrowToMachineObject(param))
            {
                _selectedMachine = (Machine)param;
                MethodName = "XCopyFolder";
                MethodParameters = "Parameters : IP Address : " + _selectedMachine.MachineIPAddress +
                    "\nSource Folder : " + _selectedMachine.LatestVersion +
                    "\nDestination Folder : " + _selectedMachine.TargetFolderPath;
                var result = await ClientHelper.XCopy();
                gRPCMethodResponse = result.ToString();
                if (result == GRPCClientHelperResponse.SUCCESS)
                    MethodError = "";
                else
                    MethodError = "UNSUCCESS"; 
            }
        }

        private async void SelfUpdate(object param)
        {
            if (ConvertDataGridrowToMachineObject(param))
            {
                _selectedMachine = (Machine)param;
                MethodName = "SelfUpdate";
                MethodParameters = "Parameters : IP Address : " + _selectedMachine.MachineIPAddress +
                    "\nSource Folder : " + _selectedMachine.LatestVersion +
                    "\nDestination Folder : " + _selectedMachine.TargetFolderPath;
                var result = await ClientHelper.SelfUpdate();
                gRPCMethodResponse = result.Message;
                if (result.Message == "SUCCESS")
                    MethodError = "";
                else
                    MethodError = "UNSUCCESS"; 
            }
        }

    }

}
