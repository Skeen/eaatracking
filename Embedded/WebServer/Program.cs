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
        private static const string ip = "10.9.8.2";
        private static const string netmask = "255.0.0.0";
        private static const string gateway = "";

        public static void Main()
        {
            // Set our devices ethernet to use a static IP
            NetworkInterface ethernet = NetworkInterface.GetAllNetworkInterfaces()[0];
            ethernet.EnableStaticIP(ip, netmask, gateway);
            // Start up our webserver on the device
            WebServer webServer = new WebServer();
            webServer.ListenForRequest();
            // We'll never reach this line.
        }

    }
}
