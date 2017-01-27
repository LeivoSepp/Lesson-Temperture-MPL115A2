using System;
using Windows.ApplicationModel.Background;
using System.Threading.Tasks;
using Microsoft.Devices.Tpm;
using Microsoft.Azure.Devices.Client;
using System.Text;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace LessonTempertureMPL115A2
{
    public sealed class StartupTask : IBackgroundTask
    {
        // MPL115A2 Sensor
        private MPL115A2 MPL115A2Sensor;
        //private void initDevice()
        //{
        //    TpmDevice device = new TpmDevice(0);
        //    string hubUri = device.GetHostName();
        //    string deviceId = device.GetDeviceId();
        //    string sasToken = device.GetSASToken();
        //    _sendDeviceClient = DeviceClient.Create(hubUri, AuthenticationMethodFactory.CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);
        //}
        //private DeviceClient _sendDeviceClient;
        //private async void SendMessages(string strMessage)
        //{
        //    string messageString = strMessage;
        //    var message = new Message(Encoding.ASCII.GetBytes(messageString));
        //    await _sendDeviceClient.SendEventAsync(message);
        //}
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            MPL115A2Sensor = new MPL115A2();
            //InitializeI2CDevice();
            //initDevice();
            while (true)
            {
                float temp = MPL115A2Sensor.getPressure();  //data[0]:pressure kPa, data[1]:temperature
                float pressure = MPL115A2Sensor.getTemperature();
                String strLux = pressure.ToString() + " " + temp.ToString();
                //SendMessages(strLux);
                Task.Delay(1000).Wait();
            }
        }
    }
}
