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
        // TODO: This needs to be updated whenever the server location changes
        private static string server_link = "http://10.11.38.141:8080/WindowsPhoneServer/";
        // userID, routeID, timeStamp, latitude, longitude
        private static string append_waypoint_page = "AppendWaypoint/";
        // userID
        private static string get_routes_page      = "GetRoutes/";
        // userID, routeID
        private static string get_waypoints_page   = "GetWaypoints/";

        // TODO: This should be loaded from a file
        private static string USER_ID = "skeen";

        public static void sendData(string routeID, string timestamp, string latitude, string longitude)
        {
            string request_url = server_link + append_waypoint_page;
            string url_rewriting = "?userID=" + USER_ID
                                 + "&routeID=" + routeID
                                 + "&timeStamp=" + timestamp
                                 + "&latitude=" + latitude
                                 + "&longitude=" + longitude
                                 ;
            string request_link = request_url + url_rewriting;

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
