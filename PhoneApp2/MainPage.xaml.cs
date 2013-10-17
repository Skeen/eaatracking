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
            // Give it a name
            locationPushpin.Tag = "locationPushpin";
            // And a location
            locationPushpin.Location = location;
            // And add it to our map
            map1.Children.Add(locationPushpin);
            // Also focus our view on the pin
            map1.SetView(location, ri.checkZoom(location, timestamp));
        }

        public void active_fade_button(Button b, bool enabled)
        {
            if (enabled)
            {
                b.Opacity = 1;
            }
            else
            {
                b.Opacity = 0.5;
            }
            b.IsEnabled = enabled;
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
            active_fade_button(stopButton, false);
            active_fade_button(pauseButton, false);
            active_fade_button(uploadButton, false);
        }

        /* Handle the information given when the start button is pressed. 
         * Currently starting the tracking */
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ri.isPaused())
            {
                // Clears the data and makes it ready for the next run.
                ri.clear(); 
                
                // Starts tracking!
                ri.startWatcher();

                // Greys out unneeded and unreachable buttons
                active_fade_button(stopButton, true);
                active_fade_button(pauseButton, true);
                active_fade_button(startButton, false);
                active_fade_button(downloadButton, false);
                active_fade_button(uploadButton, false);
            }
        }

        /* Handle the information given when the pause button is pressed.
         * Currently pausing the run, and enable the user to resume when clicked again */
        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ri.isTracking())
            {
                if (ri.isPaused())
                {
                    // Greys out the unneeded and unreachable buttons
                    active_fade_button(stopButton, true);

                    ri.unpause();

                    // Updates the app with appropiate information
                    pauseButton.Content = "Pause";
                    infoBlock.Text += "Run resumed\n";
                }
                else
                {
                    // Greys out the unneeded and unreachable buttons
                    active_fade_button(stopButton, false);

                    ri.pause();

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
            if (ri.isTracking() && !ri.isPaused())
            {
                ri.stopWatcher();
                
                // Update the app with the appropiate information once the run is over.
                infoBlock.Text = "Run is over!\n" +
                                 "Time: " + Math.Round(ri.getTimePassed().TotalMinutes) + "mins\n" +
                                 "Distance: " + Math.Round(ri.getDistanceTraveled()) + "m";

                // Greys out unneeded and unreachable buttons
                active_fade_button(stopButton, false);
                active_fade_button(pauseButton, false);
                active_fade_button(startButton, true);
                active_fade_button(downloadButton, true);
                active_fade_button(uploadButton, true);
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
            this.Content = new SavePage(ri.listGPSPositions());
        }

        public void changeTextInInfoBlock(string s1)
        {
            infoBlock.Text = s1;
        }
    }
}
