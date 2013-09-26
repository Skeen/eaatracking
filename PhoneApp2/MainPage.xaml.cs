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
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default)
                   {
                       MovementThreshold = 20
                   };
            watcher.PositionChanged += this.watcher_PositionChanged;
        }

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
    }
}