# Temperature and Pressure Sensor Adafruit MPL115A2
This project is an example on how to use the Adafruit MPL115A2 Barometric Pressure + Temperature Sensor in Raspberry PI and Windows 10 IoT Core.

## What is this sensor?
This sensor is great for all sorts of weather/environmental sensing.
This pressure sensor is a great low-cost sensing solution for measuring barometric pressure with 1.5 hPa resolution.

https://www.adafruit.com/products/992

![image](https://cloud.githubusercontent.com/assets/13704023/22854130/d067997c-f06f-11e6-801f-9c36eb7833a7.png)

Temperature is calculated in degrees C, you can convert this to F by using the classic F = C * 9/5 + 32 equation.

Pressure is returned in the SI units of Pascals. 100 Pascals = 1 hPa = 1 millibar.
As thi is not very precise sensor, it is not recommended to use for altimeter.

## How to connect this sensor into Raspberry PI?
To connect this sensor to Raspberry PI you need 4 wires. Two of the wires used for voltage VDD (+3V from Raspberry) and ground GND and remaining two are used for data. 
As this is digital sensor, it uses I2C protocol to communicate with the Raspberry. For I2C we need two wires, Data (SDA) and Clock (SCL).
Connect sensor SDA and SCL pins accordingly to Raspberry SDA and SCL pins. 

![image](https://cloud.githubusercontent.com/assets/13704023/22856431/e6cee556-f099-11e6-8d96-7d2e1baf3985.png)

## How do I write the code?
I made it very simple for you. You just need to add NuGet package RobootikaCOM.MPL115A2 to your project and you are almost done :)

After adding these NuGet packages, you just need to write 2 lines of code.

1. Create an object for sensor: 
````C#
        private MPL115A2 MPL115A2Sensor = new MPL115A2();
````

2. Write a while-loop, to read data from the sensor every 1 sec.
````C#
            while (true)
            {
                double pressure = MPL115A2Sensor.getPressure(); //kPa
                double temperature = MPL115A2Sensor.getTemperature();
                Task.Delay(1000).Wait();
            }
````
Final code looks like this. 

If you run it, you do not see anything, because it just reads the data, but it doesnt show it anywhere.
You need to integrate this project with my other example, where I teach how to send this data into Azure.

````C#
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

````

## Can I change I2C address?
I2C address is used to communicate with the sensor. Many sensors have I2C address hardcoded, and this sensor exactly this. 
**You cannot change I2C address, it is fixed to 0x60.**

## Technical Details

* PCB weight: 0.61g
* Vin: 2.4 to 5.5 VDC
* Logic: 3 to 5V compliant
* Pressure sensing range: 500-1150 hPa (up to 10Km altitude)
* 1.5 hPa / 50 m altitude resolution
* This board/chip uses I2C 7-bit address 0x60