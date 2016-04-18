using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace SensorDataCollector
{
    public sealed class StartupTask : IBackgroundTask
    {
        #region --- CONSTANT ---
        private static readonly int PERIODINSECONDS = 10;
        private static readonly string DeviceId = System.Guid.NewGuid().ToString().ToLower().Replace("-", string.Empty);
        private static readonly string connectionString = "HostName=miclHackathon0412.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=c/8zQcRwkbJ7Kh43OyaO+dxeAR8Tnvduh+wgd0JxrZc="; // ! put in value !
        #endregion
        #region --- private memebers ---
        private BackgroundTaskDeferral _deferral = null;
        private ThreadPoolTimer _timer = null;
        private DeviceClient _deviceClient = null;
        // for testing.
        private Random _rand = new Random();
        #endregion

        #region --- Constructors ---
        public StartupTask()
        {
        }
        #endregion


        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //
            _deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;
            InitDeviceClient();
            InitSensors();
            _timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Elapsed, TimeSpan.FromSeconds(PERIODINSECONDS));

            _deferral.Complete();
        }

        private async void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _timer.Cancel();
            await _deviceClient.CloseAsync();
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
        private  async Task SendDeviceToCloudMessagesAsync(string message)
        {
            await SendDeviceToCloudMessagesAsync(Encoding.ASCII.GetBytes(message));
        }
        private  async Task SendDeviceToCloudMessagesAsync(byte[] message)
        {
            if (null == message)
                return;
            var sendMessage = new Message(message);
            await _deviceClient.SendEventAsync(sendMessage);
        }
        #endregion

        #region --- Initializations ---
        private void InitDeviceClient()
        {
            _deviceClient = DeviceClient.CreateFromConnectionString(connectionString, DeviceId, TransportType.Http1);
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
