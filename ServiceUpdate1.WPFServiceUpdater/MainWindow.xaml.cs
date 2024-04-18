using ServiceUpdate1.WPFServiceUpdater.Models;
using ServiceUpdate1.WPFServiceUpdater.ViewModels;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServiceUpdate1.WPFServiceUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.DataContext = new MainViewModel();
        }

        public void btnView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Machine machine = (Machine)((Button)e.Source).DataContext;
                String MachineName = machine.MachineName;
                String MachineIPAddress = machine.MachineIPAddress;
                MessageBox.Show("You Clicked : " + MachineName + "\r\nIP Address : " + MachineIPAddress);
                //This is the code which will show the button click row data. Thank you.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}