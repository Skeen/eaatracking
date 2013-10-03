using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Microsoft.SPOT.Net.NetworkInformation;

namespace WebServer
{
    public class WebServer : IDisposable
    {
        private TempratureSensor temp_sensor;
        private Socket socket = null;
        //open connection to onbaord led so we can blink it with every request
        private OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);

        public WebServer(TempratureSensor temp_sensor_input)
        {
            // Pass our argument to an internal parameter
            temp_sensor = temp_sensor_input;
            // Initialize Socket class
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Request and bind to an IP from DHCP server
            socket.Bind(new IPEndPoint(IPAddress.Any, 80));
            // Debug print our IP address
            Debug.Print(NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress);
            // Start listen for web requests
            socket.Listen(10);
        }

        public void ListenForRequest()
        {
            // Loop forever (serve forever)
            while (true)
            {
                // Accept a Client
                using (Socket clientSocket = socket.Accept())
                {
                    // Get clients IP
                    IPEndPoint clientIP = clientSocket.RemoteEndPoint as IPEndPoint;
                    EndPoint clientEndPoint = clientSocket.RemoteEndPoint;
                    // int byteCount = cSocket.Available;
                    int bytesReceived = clientSocket.Available;
                    if (bytesReceived > 0)
                    {
                        // Get request
                        byte[] buffer = new byte[bytesReceived];
                        int byteCount = clientSocket.Receive(buffer, bytesReceived, SocketFlags.None);
                        string request = new string(Encoding.UTF8.GetChars(buffer));
                        Debug.Print(request);
                        // Compose a response
                        string response = temp_sensor.read();
                        string header = "HTTP/1.0 200 OK\r\nContent-Type: text; charset=utf-8\r\nContent-Length: " + response.Length.ToString() + "\r\nConnection: close\r\n\r\n";
                        clientSocket.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
                        clientSocket.Send(Encoding.UTF8.GetBytes(response), response.Length, SocketFlags.None);
                        // Write the debug log
                        Debug.Print("GOT REQUEST AND SEND RESPONSE");
                        // Blink the onboard LED
                        led.Write(true);
                        Thread.Sleep(150);
                        led.Write(false);
                    }
                }
            }
        }

        ~WebServer()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (socket != null)
                socket.Close();
            temp_sensor.Dispose();
        }
    }
}