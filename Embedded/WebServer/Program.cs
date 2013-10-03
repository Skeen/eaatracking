using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Microsoft.SPOT.Net.NetworkInformation;

namespace WebServer
{
    public class Program
    {
        private const string ip = "10.9.8.2";
        private const string netmask = "255.255.255.0";
        private const string gateway = "";

        public static void Main()
        {
            // Set our devices ethernet to use a static IP
            NetworkInterface ethernet = NetworkInterface.GetAllNetworkInterfaces()[0];
            ethernet.EnableStaticIP(ip, netmask, gateway);
            // Setup our Temprature Sensor
            TempratureSensor temp_sensor = new TempratureSensor();
            // Start up our webserver on the device
            WebServer webServer = new WebServer(temp_sensor);
            webServer.ListenForRequest();
            // We'll never reach this line.
        }

    }
}
