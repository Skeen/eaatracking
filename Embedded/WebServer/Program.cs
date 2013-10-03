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
    public class Program
    {
        public static void Main()
        {
            //Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].EnableDhcp();
            Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].EnableStaticIP("10.9.8.2", "255.255.255.0", "");
            WebServer webServer = new WebServer();
            webServer.ListenForRequest();
        }

    }
}
