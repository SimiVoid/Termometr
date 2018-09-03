using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO.Ports;
using System.Globalization;

namespace Termometr
{ 
    public partial class MainWindow : Window
    {
        SerialPort port;

        DispatcherTimer timer;
        TimeSpan time;

        public float minTemp = 0.0f;
        public float maxTemp = 0.0f;
        public float nowTemp = 0.0f;

        public MainWindow()
        {
            InitializeComponent();

            Connect();

            port.DataReceived += Read;

            time = TimeSpan.FromSeconds(10);

            timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                min.Content = "Min temperature: " + minTemp.ToString() + " °C";
                max.Content = "Max temperature: " + maxTemp.ToString() + " °C";
                current.Content = nowTemp.ToString() + " °C";

            }, Application.Current.Dispatcher);

            timer.Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (port.IsOpen)
            {
                port.Close();
            }
        }

        void Connect()
        {
            string[] ports = SerialPort.GetPortNames();

            foreach(string name in ports)
            {
                if(name.ToLower().StartsWith("com"))
                {
                    port = new SerialPort(name, 9600);

                    port.Open();

                    break;
                }
            }

            if(port == null)
            {
                if(MessageBox.Show("Device is not detect!", "Error!") == MessageBoxResult.OK)
                {
                    Close();
                }
            }
        }

        void Read(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string buffer = port.ReadLine();

                try
                {
                    nowTemp = float.Parse(buffer, CultureInfo.InvariantCulture.NumberFormat);
                }
                catch
                {

                }

                if(nowTemp < 200.0f)
                {
                    if (maxTemp == 0.0f && minTemp == 0.0f)
                    {
                        maxTemp = nowTemp;
                        minTemp = nowTemp;
                    }
                    else
                    {
                        if (nowTemp > maxTemp)
                            maxTemp = nowTemp;

                        if (nowTemp < minTemp)
                            minTemp = nowTemp;
                    }
                }
            }
            catch
            {
                if(MessageBox.Show("Device is not available.", "Error!") == MessageBoxResult.OK)
                {
                    Close();
                }
            }
        }
    }
}
