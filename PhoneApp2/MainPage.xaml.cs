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
            var epl = e.Position.Location;
            textBlock1.Text = textBlock1.Text + epl.Latitude.ToString("0.000") + epl.Longitude.ToString("0.000") + "\n";
        }
    }
}