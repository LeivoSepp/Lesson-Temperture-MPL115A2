using System;
using Windows.ApplicationModel.Background;
using System.Threading.Tasks;
using Glovebox.Graphics.Drivers;
using Glovebox.Graphics.Components;

namespace LessonTempertureMPL115A2
{
    public sealed class StartupTask : IBackgroundTask
    {
        // MPL115A2 Sensor
        private MPL115A2 MPL115A2Sensor;
        //LED matrix
        Ht16K33 driver = new Ht16K33(new byte[] { 0x70 }, Ht16K33.Rotate.None);

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            MPL115A2Sensor = new MPL115A2();
            LED8x8Matrix matrix = new LED8x8Matrix(driver);
            while (true)
            {
                var message = $"{Math.Round(MPL115A2Sensor.getPressure(), 1)}kPa, {Math.Round(MPL115A2Sensor.getTemperature(), 1)}C ";
                matrix.ScrollStringInFromRight(message, 70);
                Task.Delay(1000).Wait();
            }
        }
    }
}
