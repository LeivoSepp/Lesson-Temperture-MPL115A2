using System;
using Windows.ApplicationModel.Background;
using Windows.Devices.I2c;
using Windows.Devices.Enumeration;
using System.Threading.Tasks;
using Microsoft.Devices.Tpm;
using Microsoft.Azure.Devices.Client;
using System.Text;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace LessonTempertureMPL115A2
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const string I2C_CONTROLLER_NAME = "I2C1";
        private I2cDevice I2CDev;

        // MPL115A2 Sensor
        private MPL115A2 MPL115A2Sensor;

        private async void InitializeI2CDevice()
        {
            try
            {
                // Initialize I2C device 
                var settings = new I2cConnectionSettings(MPL115A2.MPL115A2_ADDRESS);
                settings.BusSpeed = I2cBusSpeed.FastMode;
                settings.SharingMode = I2cSharingMode.Shared;

                string aqs = I2cDevice.GetDeviceSelector(I2C_CONTROLLER_NAME);
                var dis = await DeviceInformation.FindAllAsync(aqs);
                I2CDev = await I2cDevice.FromIdAsync(dis[0].Id, settings);
            }
            catch (Exception e)
            {
                return;
            }
            initializeSensor();
        }
        private void initializeSensor()
        {
            MPL115A2Sensor = new MPL115A2(ref I2CDev);
        }
        private void initDevice()
        {
            TpmDevice device = new TpmDevice(0);
            string hubUri = device.GetHostName();
            string deviceId = device.GetDeviceId();
            string sasToken = device.GetSASToken();
            _sendDeviceClient = DeviceClient.Create(hubUri, AuthenticationMethodFactory.CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);
        }
        private DeviceClient _sendDeviceClient;
        private async void SendMessages(string strMessage)
        {
            string messageString = strMessage;
            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            await _sendDeviceClient.SendEventAsync(message);
        }
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            InitializeI2CDevice();
            initDevice();
            while (true)
            {
                float[] Data = MPL115A2Sensor.getPT();  //data[0]:pressure kPa, data[1]:temperature
                float press = Data[0];
                float temp = Data[1];
                String strLux = press.ToString() + " " + temp.ToString();
                SendMessages(strLux);
                Task.Delay(1000).Wait();
            }
        }
    }
}
