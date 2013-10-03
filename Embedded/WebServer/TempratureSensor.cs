using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace WebServer
{
    class TempratureSensor : IDisposable
    {
        private static const UInt16 sensor_address = 0x4F;
        private static const int sensor_clock_rate = 100;
        private static const int read_time_out = 100;

        private I2CDevice sensor;
        private I2CDevice.I2CTransaction[] read_transaction;
        private byte[] buffer_temp = new byte[2];

        public TempratureSensor()
        {
            // Create the sensor, using the given parameters
            sensor = new I2CDevice(new I2CDevice.Configuration(sensor_address, sensor_clock_rate));
            // Setup how each read transaction is the be called
            read_transaction = new I2CDevice.I2CTransaction[] { 
                // NOTE: Uncomment the below, if we're going to allow anything but reading.
                //       The write simply sets the registre for read.
                // I2CDevice.CreateWriteTransaction(new Byte[] { 0x00 }), 
                I2CDevice.CreateReadTransaction(buffer_temp)
            };
        }

        protected void do_internal_read()
        {
            // TODO; Replace sleep with timestamping, for each call
            Thread.Sleep(100);
            // Read the sensor
            int bytes_read = sensor.Execute(read_transaction, 100);
            // TODO: Throw an exception if bytes_read < 2
            Debug.Print("bytes_read=" + bytes_read + " temp = " + buffer_temp[0] + ".." + buffer_temp[1]);
        }

        public String read()
        {
            // Read the sensor
            do_internal_read();
            // Build a string of the sensor reading
            String str = "";
            str += buffer_temp[0];
            if(buffer_temp[1] != 0)
            {
                str += ".5";
            }
            return str;
        }

        ~TempratureSensor()
        {
            Dispose();
        }

        public void Dispose()
        {
            sensor.Dispose();
        }
    };
}
