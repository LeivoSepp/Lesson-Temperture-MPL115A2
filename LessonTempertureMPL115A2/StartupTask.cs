using System;
using Windows.ApplicationModel.Background;
using System.Threading.Tasks;

namespace LessonTempertureMPL115A2
{
    public sealed class StartupTask : IBackgroundTask
    {
        // MPL115A2 Sensor
        private MPL115A2 MPL115A2Sensor = new MPL115A2();

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            while (true)
            {
                double pressure = MPL115A2Sensor.getPressure(); //kPa
                double temperature = MPL115A2Sensor.getTemperature();
                Task.Delay(1000).Wait();
            }
        }
    }
}
