using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace NetduinoApplication1
{
    public class Program
    {
        public static void panic(String str)
        {
            Debug.Print(str);
            while (true)
            {
                Thread.Sleep(1000);
                Debug.Print(".");
            }
        }
        /*
        public static I2CDevice setupSensor()
        {
            const int device_address = 0x48;
            const int device_timeout = 100;

            I2CDevice.Configuration device_configuration = new I2CDevice.Configuration(device_address, device_timeout);
            I2CDevice tempSensor = new I2CDevice(device_configuration);
            
            I2CDevice.I2CTransaction[] settings = new I2CDevice.I2CTransaction[]
            {
                I2CDevice.CreateWriteTransaction(new Byte[] { 0x01, 0x60 }),
            };

            int bytes_transfered = tempSensor.Execute(settings, 100);
            if(bytes_transfered != 2)
            {
                Debug.Print("Couldn't initialize sensor");
                return tempSensor;
            }
           
            return tempSensor;
        }

        public static float read_temp(I2CDevice sensor)
        {
            Byte[] rxbuffer = new Byte[2];
            I2CDevice.I2CTransaction[] rd = new I2CDevice.I2CTransaction[]
						{
							I2CDevice.CreateWriteTransaction(new Byte[] { 0x00 }),
							I2CDevice.CreateReadTransaction(rxbuffer)
						};

            int bytes_recieved = sensor.Execute(rd, 100);
            if (bytes_recieved != 2)
            {
                Debug.Print("Couldn't read the sensor");
            }

            int tempReg = rxbuffer[0];
            tempReg = tempReg << 8;
            tempReg += rxbuffer[1];
            tempReg = tempReg >> 4;
            float tempValue = ((float)tempReg / 16);
            // Return the value
            return tempValue;
        }
        */

        public static void Main()
        {
            /*
            I2CDevice sensor = setupSensor();

            while (true)
            {
                Thread.Sleep(1000);
                float temp = read_temp(sensor);
                Debug.Print("temp = " + temp);
            }
            */
            I2CDevice ipTemp = new I2CDevice(new I2CDevice.Configuration(0x4F, 100));
            
            while (true)
            {
                
                byte[] bufTemp = new byte[2];

                I2CDevice.I2CTransaction[] readTemp = new I2CDevice.I2CTransaction[] { 
                //I2CDevice.CreateWriteTransaction(new Byte[] { 0x00 }), 
                I2CDevice.CreateReadTransaction(bufTemp)
                };
                int bytes_read = ipTemp.Execute(readTemp, 100);
                Debug.Print("bytes_read=" + bytes_read + " temp = " + bufTemp[0] + ".." + bufTemp[1]);

                Thread.Sleep(100);
            }
            ipTemp.Dispose();
        }
    }
}
