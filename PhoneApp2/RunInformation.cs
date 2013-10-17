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
        private MainPage mp;
        private bool tracking;
        private bool paused;
        private DateTimeOffset startTime; // This time is offset if the run is paused.
        private DateTimeOffset pauseTime; // The time at which the tracking was paused.
        private TimeSpan timePassed;      // Time passed sine startTime
        private Double distanceTraveled;  // The distance traveled since the start
        private List<PositionInformation> currentRunPositions;
        private PositionInformation lastKnownLocation; // TODO: Pick this out as the last element of currentRunPositions
        private GeoCoordinateWatcher watcher;
        private double last_currentSpeed;

        public List<PositionInformation> listGPSPositions()
        {
            return currentRunPositions;
        }

        public TimeSpan getTimePassed()
        {
            //TODO: Calculate whenever nedded
            return timePassed;
        }

        public double getDistanceTraveled()
        {
            //TODO: Calculate whenever nedded
            return distanceTraveled;
        }

        public RunInformation(MainPage mp) 
        { 
            /* Handle the logic of our app */
            this.mp = mp;

            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default)
            {
                MovementThreshold = 5
            };
            watcher.PositionChanged += this.watcher_PositionChanged;
            watcher.PositionChanged += this.watcher_PositionChanged2;
        }

        public void startWatcher()
        {
            watcher.Start();
            tracking = true;
        }

        public void stopWatcher()
        {
            watcher.Stop();
            tracking = false;
        }

        public bool isTracking()
        {
            return tracking;
        }

        /* Method to change your position on the map, when the phone move positon. */
        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // Checks if we actually have a position, otherwise shows a message.
            if (e.Position.Location.IsUnknown)
            {
                MessageBox.Show("Please wait while your position is determined....");
                return;
            }
            if (!paused)
            {
                // Get the current location and adds it to the current run list
                handleRunRoute(e.Position.Location, e.Position.Timestamp);

                // Centers our map on the new position. 
                mp.center_map(e.Position.Location.Latitude, e.Position.Location.Longitude);
                mp.clear_pushpins();
                mp.set_pushpin(e.Position.Location, e.Position.Timestamp);
                mp.focus_pushpin(e.Position.Location, e.Position.Timestamp);
            }
        }

        private void watcher_PositionChanged2(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            //We are running and the pause button is not pushed
            if (lastKnownLocation != null && !paused)
            {
                //Setting variables for calculations
                var lastKnownLocationPosition = new GeoCoordinate(lastKnownLocation.latitude, lastKnownLocation.longitude);
                var lastKnownLocationTime = lastKnownLocation.timeStamp;
                var currentPosition = e.Position.Location;
                var currentTime = e.Position.Timestamp;

                //Calculate distance and time from last known position
                var distanceFromLastKnownLocation = lastKnownLocationPosition.GetDistanceTo(currentPosition);
                var timeFromLastKnownLocation = currentTime.Subtract(lastKnownLocationTime);

                //Calculate the current speed
                double currentSpeed = (distanceFromLastKnownLocation / timeFromLastKnownLocation.TotalSeconds) * 3.6;

                //Calculate the total distance traveled and the average speed of the run in total
                distanceTraveled = Math.Abs(distanceFromLastKnownLocation) + Math.Abs(distanceTraveled);
                var totalTimePassedSinceStart = currentTime.Subtract(startTime);
                double averageSpeed = ((distanceTraveled / totalTimePassedSinceStart.TotalSeconds) * 3.6);
                var s1 = "Current speed: " + Math.Round(currentSpeed).ToString() + " km/h\n" +
                    "Average speed: " + Math.Round(averageSpeed).ToString() + " km/h\n";
                mp.changeTextInInfoBlock(s1);

                //Setting variables for next iteration
                lastKnownLocation = new PositionInformation(e.Position.Timestamp, e.Position.Location);
                timePassed = totalTimePassedSinceStart;
                last_currentSpeed = currentSpeed;
            }
            else if (!paused)
            {
                // If no position was found, its the first reading and we have nothing to compare to
                // So we add this location, and are ready for the next move.
                lastKnownLocation = new PositionInformation(e.Position.Timestamp, e.Position.Location);
                startTime = e.Position.Timestamp;
            }
            //We have pushed the pause button
            else
            {
                lastKnownLocation = new PositionInformation(e.Position.Timestamp, e.Position.Location);
            }
        }

        public void clear()
        {
            distanceTraveled = 0.0;
            timePassed = new TimeSpan(0,0,0);
            currentRunPositions = new List<PositionInformation>();    
        }

        // Adjust the startTime offset by the length of the pause.
        internal void unpause()
        {
            startTime.Add(DateTimeOffset.Now.Subtract(pauseTime));
            paused = false;
        }

        // Called when a pause is initiated
        internal void pause()
        {
            pauseTime = DateTimeOffset.Now;
            paused = true;
        }

        public bool isPaused()
        {
            return paused;
        }

        internal void addToCurrentRunPositions(PositionInformation positionInformation)
        {
            currentRunPositions.Add(positionInformation);
        }

        /* Method to handle the adding the entire run to a list*/
        internal void handleRunRoute(GeoCoordinate location, DateTimeOffset timestamp)
        {
            var ePosLatitude         = location.Latitude;
            var ePosLongitude        = location.Longitude;
            DateTimeOffset timeStamp = timestamp.DateTime;
            currentRunPositions.Add(new PositionInformation(timeStamp, ePosLatitude, ePosLongitude));
        }

        internal double checkZoom(GeoCoordinate location, DateTimeOffset timestamp)
        {
            //Default zoom level
            double zoomNum = 16;

            //returning a proper zoom level
            zoomNum = 20 - (last_currentSpeed * 0.05);
            if (zoomNum > 16) zoomNum = 16;
            return zoomNum;
        }
    }
}
