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
using System.Threading;

using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace PhoneApp2
{
    public class OutputToServer
    {
        // TODO: This needs to be updated whenever the server location changes
        private static readonly string server_link = "http://10.192.75.175:8080/WindowsPhoneServer/";
        // userID, routeID, timeStamp, latitude, longitude
        private static readonly string append_waypoint_page = server_link + "AppendWaypoint/";
        // userID
        private static readonly string get_routes_page = server_link + "GetRoutes/";
        // userID, routeID
        private static readonly string get_waypoints_page = server_link + "GetWaypoints/";

        // TODO: This should be loaded from a file
        private static readonly string USER_ID = "skeen";

        public delegate void listWaypointsCallbackFunction(List<PositionInformation> list);
        public static void listWaypoints(string routeID, listWaypointsCallbackFunction callback)
        {
            // Get the server page address
            string request_url = get_waypoints_page;
            // Write the url-rewriting
            string url_rewriting = "?userID=" + USER_ID
                                 + "&routeID=" + routeID
                                 ;
            // Combine to make a single request link
            string request_link = request_url + url_rewriting;
            // Create a HTTP Request, which fires the request link
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(request_link);
            // Create a list to hold the output;
            List<PositionInformation> waypoints = new List<PositionInformation>();
            // Start the request asyncly (The lambda gets called when its done)
            IAsyncResult async = request.BeginGetResponse((IAsyncResult result) =>
            {
                // This lambda simply consumes the entire reply, line by line, appending each line to the routeID list.
                HttpWebRequest http_request = result.AsyncState as HttpWebRequest;
                if (http_request != null)
                {
                    try
                    {
                        //System.Diagnostics.Debug.WriteLine("0");
                        // Get the response body
                        WebResponse response = http_request.EndGetResponse(result);
                        // Get a stream to the response
                        Stream s = response.GetResponseStream();
                        // Open a reader to that stream
                        using (StreamReader sr = new StreamReader(s))
                        {
                            //System.Diagnostics.Debug.WriteLine("1");
                            // While there are more lines
                            while (sr.Peek() >= 0)
                            {
                                // Read the first line
                                string str = sr.ReadLine();
                                // Split it on tabs
                                String[] strs = str.Split(new Char[]{'\t'});
                                // The layout is the following;
                                // strs[0] = timestamp
                                // strs[1] = latitude
                                // strs[2] = longitude
                                if (strs.Length != 3)
                                {
                                    System.Diagnostics.Debug.WriteLine("Invalid server response");
                                    continue;
                                }
                                //System.Diagnostics.Debug.WriteLine("2");
                                // Parse the strings
                                DateTimeOffset dto = DateTimeOffset.Parse(strs[0]);
                                double latitude  = Double.Parse(strs[1], NumberStyles.Float);
                                double longitude = Double.Parse(strs[2], NumberStyles.Float);
                                // Create a position
                                PositionInformation p = new PositionInformation(dto, latitude, longitude);
                                // add it to the list
                                waypoints.Add(p);
                            }
                        }
                        //System.Diagnostics.Debug.WriteLine("3");
                        callback(waypoints);
                    }
                    catch (WebException)
                    {
                        return;
                    }
                }
            }, request);
        }

        public delegate void listRoutesCallbackFunction(List<String> list);
        public static void listRoutes(listRoutesCallbackFunction callback)
        {
            // Get the server page address
            string request_url = get_routes_page;
            // Write the url-rewriting
            string url_rewriting = "?userID=" + USER_ID;
            // Combine to make a single request link
            string request_link = request_url + url_rewriting;
            // Create a HTTP Request, which fires the request link
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(request_link);
            // Start the request.
            IAsyncResult async = request.BeginGetResponse((IAsyncResult result) =>
            {
                // Create a list to hold the output;
                List<string> routeIDs = new List<string>();
                // This lambda simply consumes the entire reply, line by line, appending each line to the routeID list.
                HttpWebRequest http_request = result.AsyncState as HttpWebRequest;

                if (http_request != null)
                {
                    try
                    {
                        // Get the response
                        WebResponse response = http_request.EndGetResponse(result);
                        // Get the response stream
                        Stream s = response.GetResponseStream();
                        // Read the response stream
                        using (StreamReader sr = new StreamReader(s))
                        {
                            // While there's more to read
                            while (sr.Peek() >= 0)
                            {
                                // Read a line
                                string str = sr.ReadLine();
                                System.Diagnostics.Debug.WriteLine(str);
                                // Add it to our list
                                routeIDs.Add(str);
                            }
                        }
                        // do the callback
                        callback(routeIDs);
                        System.Diagnostics.Debug.WriteLine("1337");
                    }
                    catch (WebException)
                    {
                        return;
                    }
                }
            }, request);
        }

        public static void sendData(string routeID, PositionInformation p)
        {
            string timestamp_specifier = "s";
            string timestamp = p.timeStamp.ToString(timestamp_specifier);
            string latitude = p.latitude.ToString("G");
            string longitude = p.longitude.ToString("G");
            sendData_worker(routeID, timestamp, latitude, longitude);
        }

        private static void sendData_worker(string routeID, string timestamp, string latitude, string longitude)
        {
            // Get the server page address
            string request_url = append_waypoint_page;
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
                        // Consume the response
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
