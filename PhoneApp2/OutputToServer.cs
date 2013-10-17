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

using System.Collections.Generic;
using System.IO;

namespace PhoneApp2
{
    public class OutputToServer
    {
        // TODO: This needs to be updated whenever the server location changes
        private static readonly string server_link = "http://10.11.38.141:8080/WindowsPhoneServer/";
        // userID, routeID, timeStamp, latitude, longitude
        private static readonly string append_waypoint_page = "AppendWaypoint/";
        // userID
        private static readonly string get_routes_page = "GetRoutes/";
        // userID, routeID
        private static readonly string get_waypoints_page = "GetWaypoints/";

        // TODO: This should be loaded from a file
        private static readonly string USER_ID = "skeen";

        public static List<string> listRoutes()
        {
            // Get the server page address
            string request_url = server_link + get_routes_page;
            // Write the url-rewriting
            string url_rewriting = "?userID=" + USER_ID;
            // Combine to make a single request link
            string request_link = request_url + url_rewriting;
            // Create a HTTP Request, which fires the request link
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(request_link);
            // Create a list to hold the output;
            List<string> routeIDs = new List<string>(); 
            // Start the request asyncly (The lambda gets called when its done)
            IAsyncResult async = request.BeginGetResponse((IAsyncResult result) =>
            {
                // This lambda simply consumes the entire reply, line by line, appending each line to the routeID list.
                HttpWebRequest http_request = result.AsyncState as HttpWebRequest;
                if (http_request != null)
                {
                    try
                    {
                        WebResponse response = http_request.EndGetResponse(result);
                        Stream s = response.GetResponseStream();

                        using (StreamReader sr = new StreamReader(s))
                        {
                            while (sr.Peek() >= 0)
                            {
                                string str = sr.ReadLine();
                                routeIDs.Add(str);
                            }
                        }
                    }
                    catch (WebException)
                    {
                        return;
                    }
                }
            }, request);

            async.AsyncWaitHandle.WaitOne();
            async.AsyncWaitHandle.Close();

            return routeIDs;
        }

        public static void sendData(string routeID, string timestamp, string latitude, string longitude)
        {
            // Get the server page address
            string request_url = server_link + append_waypoint_page;
            // Write the url-rewriting
            string url_rewriting = "?userID=" + USER_ID
                                 + "&routeID=" + routeID
                                 + "&timeStamp=" + timestamp
                                 + "&latitude=" + latitude
                                 + "&longitude=" + longitude
                                 ;
            // Combine to make a single request link
            string request_link = request_url + url_rewriting;
            // Create a HTTP Request, which fires the request link
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(request_link);
            // Start the request asyncly (The lambda gets called when its done)
            request.BeginGetResponse((IAsyncResult result) =>
            {
                // This lambda simply consumes the entire reply, doing nothing with it.
                HttpWebRequest http_request = result.AsyncState as HttpWebRequest;
                if (http_request != null)
                {
                    try
                    {
                        WebResponse response = http_request.EndGetResponse(result);
                    }
                    catch (WebException)
                    {
                        return;
                    }
                }
            }, request);
        }
    }
}
