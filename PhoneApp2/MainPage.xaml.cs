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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = textBlock1.Text + "\n" + "Start Tracking" + "\n";
            watcher.Start();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = textBlock1.Text + "\n" + "Stop Tracking" + "\n";
            watcher.Stop();
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

            map1.Center = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);

            if (map1.Children.Count != 0)
            {
                var pushpin = map1.Children.FirstOrDefault(p => (p.GetType() == typeof(Pushpin) && ((Pushpin)p).Tag == "locationPushpin"));

                if (pushpin != null)
                {
                    map1.Children.Remove(pushpin);
                }
            }

            Pushpin locationPushpin = new Pushpin();
            locationPushpin.Tag = "locationPushpin";
            locationPushpin.Location = watcher.Position.Location;
            map1.Children.Add(locationPushpin);
            map1.SetView(watcher.Position.Location, 18.0);
        }
    }
}