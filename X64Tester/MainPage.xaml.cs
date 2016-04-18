using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.System.Threading;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace X64Tester
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        #region --- CONSTANT ---
        private static readonly int PERIODINSECONDS = 10;
        private static readonly string DeviceId = "Hackathon-01";//System.Guid.NewGuid().ToString().ToLower().Replace("-", string.Empty);
        private static readonly string connectionString = "HostName=miclHackathon0412.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=c/8zQcRwkbJ7Kh43OyaO+dxeAR8Tnvduh+wgd0JxrZc="; // ! put in value !
        #endregion

        #region --- private memebers ---
       // private BackgroundTaskDeferral _deferral = null;
        private ThreadPoolTimer _timer = null;
        private DeviceClient _deviceClient = null;
        // for testing.
        private Random _rand = new Random();
        #endregion


        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Start Testing
            InitDeviceClient();
            _timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Elapsed, TimeSpan.FromSeconds(PERIODINSECONDS));
        }

        public async void Timer_Elapsed(ThreadPoolTimer timer)
        {
            int min = -5;
            int max = 40;

            // Loading data.
            SensorData data = new SensorData();
            data.AirHumidity = _rand.Next(min, max);
            data.AirTemp = _rand.Next(min, max);
            data.SoilHumidity = _rand.Next(min, max);
            data.SoilTemp = _rand.Next(min, max);

            await SendDeviceToCloudMessagesAsync(data.ToString());
        }

        #region --- DeviceClient Message ---
        private async Task SendDeviceToCloudMessagesAsync(string message)
        {
            System.Diagnostics.Debug.WriteLine("Sending Message: " + message);
            await SendDeviceToCloudMessagesAsync(System.Text.Encoding.ASCII.GetBytes(message));
        }
        private async Task SendDeviceToCloudMessagesAsync(byte[] message)
        {
            if (null == message)
                return;
            var sendMessage = new Message(message);
            await _deviceClient.SendEventAsync(sendMessage);
        }
        #endregion

        #region --- Initializations ---
        private async Task InitDeviceClient()
        {
            _deviceClient = DeviceClient.CreateFromConnectionString(connectionString, DeviceId, TransportType.Http1);
            await _deviceClient.OpenAsync();
            System.Diagnostics.Debug.WriteLine("Device ID = " + DeviceId);
        }
        private void InitSensors()
        {
            InitAirSensor();
            InitSoilSensor();
        }

        private void InitSoilSensor()
        {

        }

        private void InitAirSensor()
        {

        }
        #endregion

    }
}
