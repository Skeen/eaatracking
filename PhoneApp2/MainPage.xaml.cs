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
        private GeoPositionChangedEventArgs<GeoCoordinate> lastKnownLocation;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default)
                   {
                       MovementThreshold = 5
                   };
            watcher.PositionChanged += this.watcher_PositionChanged;
            watcher.PositionChanged += this.watcher_SpeedLevels;
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
                startButton.Opacity = 0.5;
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
                startButton.Opacity = 1.0;
            }
        }

        // Method to change your position on the map, when the phone move positon.
        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (e.Position.Location.IsUnknown)
            {
                MessageBox.Show("Please wait while your prosition is determined....");
                return;
            }
            var epl = e.Position.Location;
            textBlock1.Text = textBlock1.Text + epl.Latitude.ToString("0.000") + "\t" + epl.Longitude.ToString("0.000") + "\n";

            OutputToServer.sendData(epl.Latitude.ToString("0.000"), epl.Longitude.ToString("0.000"));

            map1.Center = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);

            if (map1.Children.Count != 0)
            {
                var pushpin = map1.Children.FirstOrDefault(p => (p.GetType() == typeof(Pushpin) && (string)((Pushpin)p).Tag == "locationPushpin"));

                if (pushpin != null)
                {
                    map1.Children.Remove(pushpin);
                }
            }

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
            if (e.Position.Location.IsUnknown)
            {
                MessageBox.Show("Please wait while your speed is determined....");
                return;
            }

            if (lastKnownLocation != null)
            {
                // Random funtions to hopefully clean up the code (somewhat)
                var oldLocation = lastKnownLocation.Position.Location;
                var oldTime     = lastKnownLocation.Position.Timestamp;
                var newLocation = e.Position.Location;
                var newTime     = e.Position.Timestamp;
                
                var distance = oldLocation.GetDistanceTo(newLocation);
                var time = newTime.Subtract(oldTime);

                var speed = Math.Round(distance / time.TotalSeconds);
                var speedKm = speed * 3.6;
                speedBox.Text = speed.ToString() + " m/sec\n" + speedKm.ToString() + " km/h";
                map1.ZoomLevel = speedKm * 0.5;
            }
            
            lastKnownLocation = e;
        }
    }
}