using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using GrovePi.I2CDevices;
using GrovePi.Sensors;
using GrovePi;
using System.Threading.Tasks;
using System.Diagnostics;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace EventPi
{
    public sealed class StartupTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // LCD - This screen is I2C
            IRgbLcdDisplay LCD = DeviceFactory.Build.RgbLcdDisplay();
            LCD.SetBacklightRgb(255, 255, 255);
            LCD.SetText("Hello world!"); // Not sure what colour this will show up in

            // LEDs
            ILed red = DeviceFactory.Build.Led(Pin.DigitalPin2);
            ILed blue = DeviceFactory.Build.Led(Pin.DigitalPin3);

            // Ultrasonic 
            IUltrasonicRangerSensor Ultrasonic = DeviceFactory.Build.UltraSonicSensor(Pin.DigitalPin4);

            // Temperature and Humidity
            // TODO: Double check Sensor model number. Assumed DHT11 from the GrovePi+ Starter Kit
            IDHTTemperatureAndHumiditySensor tempHumidity = DeviceFactory.Build.DHTTemperatureAndHumiditySensor(Pin.DigitalPin5, DHTModel.Dht11);

            // Sound sensor
            ISoundSensor Sound = DeviceFactory.Build.SoundSensor(Pin.AnalogPin0);

            // LDR
            IRotaryAngleSensor LDR = DeviceFactory.Build.RotaryAngleSensor(Pin.AnalogPin1);

            while (true)
            {
                Task.Delay(100).Wait();

                try
                {
                    // Ultrasonic sensor
                    int distance = Ultrasonic.MeasureInCentimeters();
                    Debug.WriteLine("Distance: " + distance.ToString());
                    // TODO - tune to distance of door
                    //if(distance < 50)

                    // LDR
                    int lightLevel = LDR.SensorValue();
                    Debug.WriteLine("Light Level: " + lightLevel.ToString());

                    // LEDs
                    red.ChangeState(SensorStatus.On);
                    blue.ChangeState(SensorStatus.On);

                    // Temperature Humidity
                    tempHumidity.Measure();
                    double temp_degC = tempHumidity.TemperatureInCelsius;
                    double humidity = tempHumidity.Humidity;
                    Debug.WriteLine("Temperature: " + temp_degC + "\tHumidity: " + humidity);

                    // Sound sensor
                    int soundLevel = Sound.SensorValue();
                    Debug.WriteLine("Sound Level: " + soundLevel);

                    // TODO: Send data to Azure

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
