using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace arduino_pc_communication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isConnected = false;
        String[] ports;
        SerialPort port;
        DispatcherTimer timer = new DispatcherTimer();

        public enum DataSendingState
        {
            GetDate,
            GetTime,
            None
        }

        public DataSendingState dataSendingState;

        public MainWindow()
        {
            InitializeComponent();
            SetupApplication();
        }

        void SetupApplication()
        {
            GetAvailableComPorts();
            DisableControls();

            foreach (var port in ports)
            {
                comboBoxPorts.Items.Add(port);
                if (ports[0] != null)
                    comboBoxPorts.SelectedItem = ports[0];
            }

            timer.IsEnabled = false;
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 1);
        }

        void SendCommand(string command, string content)
        {
            if (content == null)
                port.Write(string.Format("#{0}\n", command));
            else
                port.Write(string.Format("#{0}{1}\n", command, content));
        }

        void GetAvailableComPorts() => ports = SerialPort.GetPortNames();

        private void ConnectToPort()
        {
            isConnected = true;
            string selectedPort = comboBoxPorts.SelectedValue.ToString().ToUpper();
            portsList.Content = string.Format("Connected to: {0}", selectedPort);
            port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
            port.Open();
            SendCommand("SYSCON", null);
            connectionButton.Content = "Disconnect";
            EnableControls();
        }
        private void DisconnectFromPort()
        {
            portsList.Content = "Ports to connect";
            isConnected = false;
            SendCommand("SYSDSC", null);
            port.Close();
            connectionButton.Content = "Connect";
            DisableControls();
            //reset to default
        }

        private void connectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                ConnectToPort();
                comboBoxPorts.IsEnabled = false;
            }
            else
            {
                DisconnectFromPort();
                comboBoxPorts.IsEnabled = true;
            }
        }

        private void timer_Tick(object sender, EventArgs e) => RefreshData();

        private void RefreshData()
        {
            DateTime date = DateTime.Now;
            switch (dataSendingState)
            {
                case DataSendingState.GetDate:
                    SendCommand("SYSDSP", "Date:           " + date.ToString("dd-MM-yyyy"));
                    break;
                case DataSendingState.GetTime:
                    SendCommand("SYSDSP", "Time:           " + date.ToString("HH:mm:ss"));
                    break;
                case DataSendingState.None:
                    break;
            }
        }

        private void sendText_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("SYSDSP", textToSend.Text);
        }

        private void GetDate_Click(object sender, RoutedEventArgs e)
        {
            dataSendingState = DataSendingState.GetDate;
            timer.IsEnabled = true;
        }

        private void GetTime_Click(object sender, RoutedEventArgs e)
        {
            dataSendingState = DataSendingState.GetTime;
            timer.IsEnabled = true;
        }

        private void None_Click(object sender, RoutedEventArgs e)
        {
            dataSendingState = DataSendingState.None;
            timer.IsEnabled = false;
        }

        private void EnableControls()
        {
            textToSend.IsEnabled = true;
            sendText.IsEnabled = true;
            GetDate.IsEnabled = true;
            GetTime.IsEnabled = true;
            None.IsEnabled = true;
        }

        private void DisableControls()
        {
            textToSend.IsEnabled = false;
            sendText.IsEnabled = false;
            GetDate.IsEnabled = false;
            GetTime.IsEnabled = false;
            None.IsEnabled = false;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e) => Close();
        private void miniButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    }
}