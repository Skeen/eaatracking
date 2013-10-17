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
        private RunInformation ri;
       
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            init();
        }

        public void set_pushpin(GeoCoordinate location, DateTimeOffset timestamp)
        {
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
            locationPushpin.Location = location;
            map1.Children.Add(locationPushpin);
            map1.SetView(location, ri.checkZoom(location, timestamp));
        }

        public void center_map(double latitude, double longtitude)
        {
            map1.Center = new GeoCoordinate(latitude, longtitude);
        }

        // To be called when our app starts
        private void init()
        {
            ri = new RunInformation(this);

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
                ri.startWatcher();
                

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
                ri.stopWatcher();
                
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

        public void changeTextInInfoBlock(string s1)
        {
            infoBlock.Text = s1;
        }
    }
}
