using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhoneApp2
{
    public class OutputToServer
    {
        private static string server_link = "http://127.0.0.1:8080/WindowsPhoneServer/";

        public static void sendData(string latitude, string longitude)
        {
            string request_link = server_link + "?Latitude=" + latitude + "&Longitude=" + longitude;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(request_link);
            request.BeginGetResponse(callBack, request);
        }

        public static void callBack(IAsyncResult result)
        {
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            if (request != null)
            {
                try
                {
                    WebResponse response = request.EndGetResponse(result);
                }
                catch (WebException)
                {                    
                    return;
                }
            }
        }
    }
}
