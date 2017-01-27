using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace LessonTempertureMPL115A2
{
    class MPL115A2
    {
        // Address Constant
        public const int MPL115A2_ADDRESS = 0x60;
        private const int MPL115A2_REGISTER_STARTCONVERSION = 0x12;
        private const int MPL115A2_REGISTER_PRESSURE_MSB = 0x00;
        private const int MPL115A2_REGISTER_TEMP_MSB = 0x02;
        private const int MPL115A2_REGISTER_A0_COEFF_MSB = 0x04;
        private const int MPL115A2_REGISTER_B1_COEFF_MSB = 0x06;
        private const int MPL115A2_REGISTER_B2_COEFF_MSB = 0x08;
        private const int MPL115A2_REGISTER_C12_COEFF_MSB = 0x0A;

        private float _mpl115a2_a0;
        private float _mpl115a2_b1;
        private float _mpl115a2_b2;
        private float _mpl115a2_c12;

        // I2C Device
        private I2cDevice I2C;
        private int I2C_ADDRESS { get; set; } = MPL115A2_ADDRESS;
        public MPL115A2(int i2cAddress = MPL115A2_ADDRESS)
        {
            I2C_ADDRESS = i2cAddress;
        }
        private void readCoefficients()
        {
            int a0coeff;
            int b1coeff;
            int b2coeff;
            int c12coeff;

            a0coeff = I2CRead16(MPL115A2_REGISTER_A0_COEFF_MSB);
            b1coeff = I2CRead16(MPL115A2_REGISTER_B1_COEFF_MSB);
            b2coeff = I2CRead16(MPL115A2_REGISTER_B2_COEFF_MSB);
            c12coeff = I2CRead16(MPL115A2_REGISTER_C12_COEFF_MSB) >> 2;

            _mpl115a2_a0 = (float)a0coeff / 8;
            _mpl115a2_b1 = (float)b1coeff / 8192;
            _mpl115a2_b2 = (float)b2coeff / 16384;
            _mpl115a2_c12 = (float)c12coeff;
            _mpl115a2_c12 /= (float)4194304.0;
        }
        public static bool IsInitialised { get; private set; } = false;
        private void Initialise()
        {
            if (!IsInitialised)
            {
                EnsureInitializedAsync().Wait();
            }
        }
        private async Task EnsureInitializedAsync()
        {
            if (IsInitialised) { return; }
            try
            {
                var settings = new I2cConnectionSettings(I2C_ADDRESS);
                settings.BusSpeed = I2cBusSpeed.FastMode;
                settings.SharingMode = I2cSharingMode.Shared;
                string aqs = I2cDevice.GetDeviceSelector("I2C1");         /* Find the selector string for the I2C bus controller */
                var dis = await DeviceInformation.FindAllAsync(aqs);      /* Find the I2C bus controller device with our selector string           */
                I2C = await I2cDevice.FromIdAsync(dis[0].Id, settings);   /* Create an I2cDevice with our selected bus controller and I2C settings */

                readCoefficients();
                InitiliseRegisters();
                IsInitialised = true;
            }
            catch (Exception ex)
            {
                throw new Exception("I2C Initialization Failed", ex);
            }
        }
        protected virtual void InitiliseRegisters()
        {
            write8(MPL115A2_REGISTER_STARTCONVERSION, 0x00);
        }
        private float getRawTemperature()
        {
            //read temperature RAW data from device
            return (uint)I2CRead16(MPL115A2_REGISTER_TEMP_MSB) >> 6;
        }
        public float getTemperature()
        {
            Initialise();
            float rawTemp = getRawTemperature();
            return ((float)rawTemp - 498.0F) / -5.35F + 25.0F;       // C
        }
        public float getPressure()
        {
            Initialise();
            uint pressure;
            float rawTemp = getRawTemperature();
            float pressureComp;
            //read pressure data from device
            pressure = (uint)I2CRead16(MPL115A2_REGISTER_PRESSURE_MSB) >> 6;
            // See datasheet p.6 for evaluation sequence
            pressureComp = _mpl115a2_a0 + (_mpl115a2_b1 + _mpl115a2_c12 * rawTemp) * pressure + _mpl115a2_b2 * rawTemp;
            //calculate pressure and return pressure data
            return ((65.0F / 1023.0F) * pressure) + 50.0F;        // kPa
        }
        private void write8(byte addr, byte cmd)
        {
            byte[] Command = new byte[] { (byte)(addr), cmd };
            I2C.Write(Command);
        }
        private ushort I2CRead16(byte addr)
        {
            byte[] aaddr = new byte[] { (byte)(addr) };
            byte[] data = new byte[2];

            I2C.WriteRead(aaddr, data);

            return (ushort)((data[0] << 8) | (data[1]));
        }
    }
}
