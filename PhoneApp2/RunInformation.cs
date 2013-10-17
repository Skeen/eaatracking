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
using System.Device.Location;

namespace PhoneApp2
{
    public class RunInformation
    {
        public bool tracking { get; set; }
        public bool paused { get; set; }

        public DateTimeOffset startTime { get; set; } /* This time is offset if the run is paused.*/
        private DateTimeOffset pauseTime;
        public TimeSpan timePassed { get; set; }
        public Double distanceTraveled { get; set; }
        public List<PositionInformation> currentRunPositions { get; set; }
        
        public RunInformation() { 
            /* Handle the logic of our app */ 
        }

        public void clear() {
            distanceTraveled = 0.0;
            timePassed = new TimeSpan(0,0,0);
            currentRunPositions = new List<PositionInformation>();    
        }

        // Adjust the startTime offset by the length of the pause.
        internal void adjustTimeForPause(DateTimeOffset dateTimeOffset)
        {
            startTime.Add(dateTimeOffset.Subtract(pauseTime));
            paused = false;
        }

        // Called when a pause is initiated
        internal void setPauseTime(DateTimeOffset dateTimeOffset)
        {
            pauseTime = dateTimeOffset;
            paused = true;
        }

        internal void addToCurrentRunPositions(PositionInformation positionInformation)
        {
            currentRunPositions.Add(positionInformation);
        }

        /* Method to handle the adding the entire run to a list*/
        internal void handleRunRoute(GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var ePosLatitude = e.Position.Location.Latitude;
            var ePosLongitude = e.Position.Location.Longitude;
            DateTimeOffset timeStamp = e.Position.Timestamp.DateTime;
            currentRunPositions.Add(new PositionInformation(timeStamp, ePosLatitude, ePosLongitude));
        }
    }
}
