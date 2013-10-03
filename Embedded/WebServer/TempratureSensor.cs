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
    class TempratureSensor
    {
        private I2CDevice sensor;
        private I2CDevice.I2CTransaction[] read_transaction;
        private byte[] buffer_temp = new byte[2];

        public TempratureSensor()
        {
            sensor = new I2CDevice(new I2CDevice.Configuration(0x4F, 100));
            read_transaction = new I2CDevice.I2CTransaction[] { 
                //I2CDevice.CreateWriteTransaction(new Byte[] { 0x00 }), 
                I2CDevice.CreateReadTransaction(buffer_temp)
            };
        }

        protected void do_internal_read()
        {
            // TODO; Replace sleep with timestamping, for each call
            Thread.Sleep(100);
            int bytes_read = sensor.Execute(read_transaction, 100);
            Debug.Print("bytes_read=" + bytes_read + " temp = " + buffer_temp[0] + ".." + buffer_temp[1]);
        }

        public String read()
        {
            do_internal_read();
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
