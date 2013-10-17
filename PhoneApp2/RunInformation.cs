﻿using System;
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
        private MainPage mp;
        public bool tracking { get; set; }
        public bool paused { get; set; }
        public DateTimeOffset startTime { get; set; } /* This time is offset if the run is paused.*/
        private DateTimeOffset pauseTime;
        public TimeSpan timePassed { get; set; }
        public Double distanceTraveled { get; set; }
        public List<PositionInformation> currentRunPositions { get; private set; }
        private GeoPositionChangedEventArgs<GeoCoordinate> lastKnownLocation; 
        
        public RunInformation(MainPage mp) { 
            /* Handle the logic of our app */
            this.mp = mp;
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

        internal double checkZoom(GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            //Default zoom level
            double zoomNum = 16;

            //We are running and the pause button is not pushed
            if (lastKnownLocation != null && !paused)
            {
                //Setting variables for calculations
                var lastKnownLocationPosition = lastKnownLocation.Position.Location;
                var lastKnownLocationTime = lastKnownLocation.Position.Timestamp;
                var currentPosition = e.Position.Location;
                var currentTime = e.Position.Timestamp;

                //Calculate distance and time from last known position
                var distanceFromLastKnownLocation = lastKnownLocationPosition.GetDistanceTo(currentPosition);
                var timeFromLastKnownLocation = currentTime.Subtract(lastKnownLocationTime);

                //Calculate the current speed
                var currentSpeed = (distanceFromLastKnownLocation / timeFromLastKnownLocation.TotalSeconds) * 3.6;

                //Calculate the total distance traveled and the average speed of the run in total
                distanceTraveled = Math.Abs(distanceFromLastKnownLocation) + Math.Abs(distanceTraveled);
                var totalTimePassedSinceStart = currentTime.Subtract(startTime);
                var averageSpeed = ((distanceTraveled / totalTimePassedSinceStart.TotalSeconds) * 3.6);
                var s1 = "Current speed: " + Math.Round(currentSpeed).ToString() + " km/h\n" +
                         "Average speed: " + Math.Round(averageSpeed).ToString() + " km/h\n";
                mp.changeTextInInfoBlock(s1);
                
                //Setting variables for next iteration
                lastKnownLocation = e;
                timePassed = totalTimePassedSinceStart;

                //returning a proper zoom level
                zoomNum = 20 - (currentSpeed * 0.05);
                if (zoomNum > 16) zoomNum = 16;
                return zoomNum;
            }
            //We have just pushed the start button so this is the first read of a location
            else if (!paused)
            {
                // If no position was found, its the first reading and we have nothing to compare to
                // So we add this location, and are ready for the next move.
                lastKnownLocation = e;
                startTime = e.Position.Timestamp;
                return zoomNum;
            }
            //We have pushed the pause button
            else
            {
                lastKnownLocation = e;
                return zoomNum;
            }
        }
    }
}
