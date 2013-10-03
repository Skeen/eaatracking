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

namespace PhoneApp2
{
    public partial class MainPage : PhoneApplicationPage
    {
        private GeoCoordinateWatcher watcher;
        private Boolean tracking = false;
        private Boolean paused = false;
        private GeoPositionChangedEventArgs<GeoCoordinate> lastKnownLocation;
        private DateTimeOffset startTime;
        private DateTimeOffset pauseTime;

        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            setUpRandomThings();
        }

        // To be called when our app starts, and setup most of the things.
        private void setUpRandomThings()
        {
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default)
            {
                MovementThreshold = 5
            };
            
            watcher.PositionChanged += this.watcher_PositionChanged;
            watcher.PositionChanged += this.watcher_SpeedLevels;

            stopButton.Opacity = 0.5;
            pauseButton.Opacity = 0.5;
        }

        /* Handle the information given when the start button is pressed. 
         * Currently starting the tracking */
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (!tracking)
            {
                textBlock1.Text = textBlock1.Text + "\n" + "Start Tracking" + "\n";
                watcher.Start();
                tracking = true;
                stopButton.Opacity = 1.0;
                pauseButton.Opacity = 1.0;
                startButton.Opacity = 0.5;
            }
        }

        /* Handle the information given when the pause button is pressed.
         * Currently pausing the run, and enable the user to resume when clicked again */
        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (tracking)
            {
                if (paused)
                {
                    startTime.Add(DateTimeOffset.Now.Subtract(pauseTime));
                    pauseButton.Content = "Pause";
                    infoBlock.Text += "Run resumed\n" + startTime.TimeOfDay + "\n";
                    paused = false;
                }
                else
                {
                    pauseTime = DateTimeOffset.Now;
                    paused = true;
                    pauseButton.Content = "Resume";
                    infoBlock.Text += "Run paused\n" + startTime.TimeOfDay + "\n";
                }
            }
        }

        /* Handle the information given when the stop button is pressed.
         * Currently stopping the tracking */
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (tracking)
            {
                textBlock1.Text = textBlock1.Text + "\n" + "Stop Tracking" + "\n";
                watcher.Stop();
                tracking = false;
                stopButton.Opacity = 0.5;
                pauseButton.Opacity = 0.5;
                startButton.Opacity = 1.0;
            }
        }

        // Method to change your position on the map, when the phone move positon.
        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // Checks if we actually have a position, otherwise shows a message.
            if (e.Position.Location.IsUnknown)
            {
                MessageBox.Show("Please wait while your prosition is determined....");
                return;
            }
            // Get the current location and prints it.
            var epl = e.Position.Location;
            textBlock1.Text = textBlock1.Text + epl.Latitude.ToString("0.000") + "\t" + epl.Longitude.ToString("0.000") + "\n";

            // Sends the data out
            OutputToServer.sendData(epl.Latitude.ToString("0.000"), epl.Longitude.ToString("0.000"));
            
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
            map1.SetView(watcher.Position.Location, 16.0);
        }

        /* Method to handle the speed of which our runner (walker) is moving.
         * Also handle the zoom level of the map. */
        private void watcher_SpeedLevels(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // Checks if we actually have a position, otherwise shows a message.
            if (e.Position.Location.IsUnknown)
            {
                MessageBox.Show("Please wait while your speed is determined....");
                return;
            }

            if (lastKnownLocation != null)
            {
                // Random functions to clean up the code (somewhat)
                var oldLocation = lastKnownLocation.Position.Location;
                var oldTime     = lastKnownLocation.Position.Timestamp;
                var newLocation = e.Position.Location;
                var newTime     = e.Position.Timestamp;
                
                // Calculate the difference between this and the preivous location.
                var distance = oldLocation.GetDistanceTo(newLocation);
                var time = newTime.Subtract(oldTime);

                // Calculate the speed and adjusts the map accordingly. 
                var speed = Math.Round(distance / time.TotalSeconds);
                var speedKm = speed * 3.6;
                speedBlock.Text = "Current speed: " + speed.ToString() + " m/sec\n" + speedKm.ToString() + " km/h";
                map1.ZoomLevel = 20 - (speedKm * 0.3);
            }
            
            // If no position was found, its the first reading and we have nothing to compare to
            // So we add this location, and are ready for the next move.
            lastKnownLocation = e;
            startTime = e.Position.Timestamp;
        }
    }
}