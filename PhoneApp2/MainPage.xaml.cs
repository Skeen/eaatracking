using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using System.Collections.ObjectModel;

namespace PhoneApp2
{
    public partial class MainPage : PhoneApplicationPage
    {
        ObservableCollection<string> _items = new ObservableCollection<string>();

        private GeoPositionChangedEventArgs<GeoCoordinate> lastKnownLocation;
        private GeoCoordinateWatcher watcher;
/*        private Boolean tracking = false;
        private Boolean paused = false;
        private DateTimeOffset startTime; //This time is offset if the run is paused.
        private DateTimeOffset pauseTime;
        private TimeSpan timePassed;
        private Double distanceTraveled;
        private List<PositionInformation> currentRunPositions; */

        private RunInformation ri;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            init();
        }

        // To be called when our app starts
        private void init()
        {
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default)
            {
                MovementThreshold = 5
            };

            ri = new RunInformation();
            watcher.PositionChanged += this.watcher_PositionChanged;

            // Greys out unneeded and unreachable buttons
            stopButton.Opacity = 0.5;
            stopButton.IsEnabled = false;
            pauseButton.Opacity = 0.5;
            pauseButton.IsEnabled = false;
            uploadButton.Opacity = 0.5;
            uploadButton.IsEnabled = false;
        }

        /* Handle the information given when the start button is pressed. 
         * Currently starting the tracking */
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ri.paused)
            {
                // Clears the data and makes it ready for the next run.
                ri.clear();
                
                // Starts tracking!
                watcher.Start();
                ri.tracking = true;

                // Greys out unneeded and unreachable buttons
                stopButton.Opacity = 1.0;
                stopButton.IsEnabled = true;
                pauseButton.Opacity = 1.0;
                pauseButton.IsEnabled = true;
                startButton.Opacity = 0.5;
                startButton.IsEnabled = false;
                downloadButton.Opacity = 0.5;
                downloadButton.IsEnabled = false;
                uploadButton.Opacity = 0.5;
                uploadButton.IsEnabled = false;
            }
        }

        /* Handle the information given when the pause button is pressed.
         * Currently pausing the run, and enable the user to resume when clicked again */
        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ri.tracking)
            {
                if (ri.paused)
                {
                    // Greys out the unneeded and unreachable buttons
                    stopButton.Opacity = 1.0;
                    stopButton.IsEnabled = true;

                    ri.adjustTimeForPause(DateTimeOffset.Now);

                    // Updates the app with appropiate information
                    pauseButton.Content = "Pause";
                    infoBlock.Text += "Run resumed\n";
                }
                else
                {
                    // Greys out the unneeded and unreachable buttons
                    stopButton.Opacity = 0.5;
                    stopButton.IsEnabled = false;

                    ri.setPauseTime(DateTimeOffset.Now);

                    // Updates the app with appropiate information
                    pauseButton.Content = "Resume";
                    infoBlock.Text += "Run paused\n";
                }
            }
        }

        /* Handle the information given when the stop button is pressed.
         * Currently stopping the tracking */
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (ri.tracking && !ri.paused)
            {
                watcher.Stop();
                ri.tracking = false;
                
                // Update the app with the appropiate information once the run is over.
                infoBlock.Text = "Run is over!\n" +
                                 "Time: " + Math.Round(ri.timePassed.TotalMinutes) + "mins\n" +
                                 "Distance: " + Math.Round(ri.distanceTraveled) + "m";

                // Greys out unneeded and unreachable buttons
                stopButton.Opacity = 0.5;
                stopButton.IsEnabled = false;
                pauseButton.Opacity = 0.5;
                pauseButton.IsEnabled = false;
                startButton.Opacity = 1.0;
                startButton.IsEnabled = true;
                downloadButton.Opacity = 1.0;
                downloadButton.IsEnabled = true;
                uploadButton.Opacity = 1.0;
                uploadButton.IsEnabled = true;
            }
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
            if (!ri.paused)
            {
                // Get the current location and adds it to the current run list
                var epl = e.Position.Location;
                ri.handleRunRoute(e);
                
                // Sends the data out
                /*
                var ePosLatitude = e.Position.Location.Latitude;
                var ePosLongitude = e.Position.Location.Longitude;
                DateTimeOffset timeStamp = e.Position.Timestamp.DateTime;
                PositionInformation pi = new PositionInformation(timeStamp, ePosLatitude, ePosLongitude);
                OutputToServer.sendData("rundt_om_nygaard", pi);
                */
                // Centers our map on the new position. 
                map1.Center = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);

                if (map1.Children.Count != 0)
                {
                    var pushpin = map1.Children.FirstOrDefault(p => (p.GetType() == typeof(Pushpin) && (string)((Pushpin)p).Tag == "locationPushpin"));
                    if (pushpin != null)
                    {
                        map1.Children.Remove(pushpin);
                    }
                }

                // Show our location with a pin on the map.
                Pushpin locationPushpin = new Pushpin();
                locationPushpin.Tag = "locationPushpin";
                locationPushpin.Location = watcher.Position.Location;
                map1.Children.Add(locationPushpin);
                map1.SetView(watcher.Position.Location, zoomAmount(e));
            }
        }

        

        /* Method to handle the speed of which our runner (walker) is moving.
         * Also handle the zoom level of the map. */
        private double zoomAmount(GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            //Default zoom level
            double zoomNum = 16;

            //We are running and the pause button is not pushed
            if (lastKnownLocation != null && !ri.paused)
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
                ri.distanceTraveled = Math.Abs(distanceFromLastKnownLocation) + Math.Abs(ri.distanceTraveled);
                var totalTimePassedSinceStart = currentTime.Subtract(ri.startTime);
                var averageSpeed = ((ri.distanceTraveled / totalTimePassedSinceStart.TotalSeconds) * 3.6);
                infoBlock.Text = "Current speed: " + Math.Round(currentSpeed).ToString() + " km/h\n" +
                                 "Average speed: " + Math.Round(averageSpeed).ToString() + " km/h\n";

                //Setting variables for next iteration
                lastKnownLocation = e;
                ri.timePassed = totalTimePassedSinceStart;

                //returning a proper zoom level
                zoomNum = 20 - (currentSpeed * 0.05);
                if (zoomNum > 16) zoomNum = 16;                
                return zoomNum;
            }
            //We have just pushed the start button so this is the first read of a location
            else if (!ri.paused)
            {
                // If no position was found, its the first reading and we have nothing to compare to
                // So we add this location, and are ready for the next move.
                lastKnownLocation = e;
                ri.startTime = e.Position.Timestamp;
                return zoomNum;
            }
            //We have pushed the pause button
            else
            {
                lastKnownLocation = e;
                return zoomNum;
            }
        }

        private void load_clicked(object sender, RoutedEventArgs e)
        {
            this.Content = new LoadPage();
        }

        /* Handle the information given when the upload button is pressed
         * Currently tries to upload the current route to the cloudserver */
        private void uploadButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new SavePage(ri.currentRunPositions);
        }
    }
}
