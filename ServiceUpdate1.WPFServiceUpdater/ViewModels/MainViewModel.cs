using ServiceUpdate1.GrpcClient;
using ServiceUpdate1.WPFServiceUpdater.Lib;
using ServiceUpdate1.WPFServiceUpdater.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows.Input;

namespace ServiceUpdate1.WPFServiceUpdater.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Machine> Machines { get; set; }
        private Machine _selectedMachine = null;
        public Machine SelectedMachine
        {
            get { return _selectedMachine; }
            set { _selectedMachine = value; OnPropertyChanged(nameof(SelectedMachine)); }
        }
        private GRPCClientHelper? _clientHelper = null;
        public GRPCClientHelper ClientHelper
        {
            get
            {
                return new GRPCClientHelper(IPAddress.Parse(_selectedMachine.MachineIPAddress),
                        _selectedMachine.Port,
                        _selectedMachine.InstalledFilePath,
                        _selectedMachine.LatestVersion,
                        _selectedMachine.TargetFolderPath);
            }
        }

        public MainViewModel()
        {
            var machineCollection = new MachineCollection();
            machineCollection.LoadMachines();
            Machines = machineCollection.Machines;
        }

        private string _response;
        public string Response
        {
            get { return _response; }
            set { _response = value; OnPropertyChanged(nameof(Response)); }
        }

        private string _methodName;
        public string MethodName
        {
            get { return _methodName; }
            set { _methodName = value; OnPropertyChanged(nameof(MethodName)); }
        }

        private string _parameters;
        public string Parameters
        {
            get { return _parameters; }
            set { _parameters = value; OnPropertyChanged(nameof(Parameters)); }
        }

        private string _error;
        public string Error
        {
            get { return _error; }
            set { _error = value; OnPropertyChanged(nameof(Error)); }
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
                    _updateCommand = new RelayCommand(param => UpdateService());
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
                    _getInstalledVersionCommand = new RelayCommand(param => GetInstalledVersion());
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
                    _uploadFileCommand = new RelayCommand(param => UploadFile());
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
                    _xCopyCommand = new RelayCommand(param => XCopy());
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
                    _SelfUpdateCommand = new RelayCommand(param => SelfUpdate());
                }
                return _SelfUpdateCommand;
            }
        }

        enum gRPCCalls
        {
            GetInstalledVersion,
            UpdateService,
            UploadFile,
            XCopy,
            SelfUpdate
        }

        private async void CallToGRPC(gRPCCalls getInstalledVersion)
        {
            //GrpcServer.ResponseMessage result;
            //dynamic result;
            MethodName = "";
            Parameters = "";
            Response = "";
            Error = "";
            if (SelectedMachine is null)
            {
                Error = "Please select machine first!";
                return;
            }
            var result = ClientHelper.DummyResult();
            Parameters = "Parameters : IP Address : " + _selectedMachine.MachineIPAddress +
                            "\nFile Path : " + _selectedMachine.InstalledFilePath +
                            "\nSource Folder : " + _selectedMachine.LatestVersion +
                            "\nDestination Folder : " + _selectedMachine.TargetFolderPath;
            switch (getInstalledVersion)
            {
                case gRPCCalls.GetInstalledVersion:
                    MethodName = "GetInstalledVersion";
                    result = await ClientHelper.GetFileInstalledVersion();
                    SelectedMachine.InstalledVersion = result.Message;
                    break;
                case gRPCCalls.UpdateService:
                    MethodName = "UpdateService";
                    result = await ClientHelper.SendUpdates();
                    break;
                case gRPCCalls.UploadFile:
                    MethodName = "UploadFile";
                    result = await ClientHelper.UploadFile();
                    break;
                case gRPCCalls.XCopy:
                    MethodName = "XCopy";
                    result = await ClientHelper.XCopy();
                    break;
                case gRPCCalls.SelfUpdate:
                    MethodName = "SelfUpdate";
                    result = await ClientHelper.SelfUpdate();
                    break;
                default:
                    break;
            }
            if (!result.Success)
                Error = result.Error;
            Response = result.Message;
        }

        private async void GetInstalledVersion()
        {
            CallToGRPC(gRPCCalls.GetInstalledVersion);
        }

        private async void UpdateService()
        {
            CallToGRPC(gRPCCalls.UpdateService);
        }

        private async void UploadFile()
        {
            CallToGRPC(gRPCCalls.UploadFile);
        }

        private async void XCopy()
        {
            CallToGRPC(gRPCCalls.XCopy);
        }

        private async void SelfUpdate()
        {
            CallToGRPC(gRPCCalls.SelfUpdate);
        }

    }

}
