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
using System.Windows.Controls.Primitives;

namespace PhoneApp2
{
    public partial class MainPage : PhoneApplicationPage
    {
        ObservableCollection<string> _items = new ObservableCollection<string>();
        public RunInformation ri;
        
        // Ugly pattern to ensure that Save & Load Page har access to the MainPage.
        private static MainPage mp;
        public static MainPage getSingleton()
        {
            return mp;
        }

        // Constructor
        public MainPage()
        {
            // The Main Page will only be created once, and we'll therefore
            // implement the singleton pattern in the constructor.
            mp = this;

            ri = new RunInformation(this);
            // Initialize the mainpage
            InitializeComponent();
            init();
        }

        // The tag, used for the running (user) push pin, also the default for the functions below.
        private const string default_tag = "locationPushpin";
        // Clear all pushpins, with a specific tag, from the map.
        public void clear_pushpins(string tag = default_tag)
        {
            while (map1.Children.Count != 0)
            {
                // Get a valid push pin, if any (with the specific tag)
                var pushpin = map1.Children.FirstOrDefault(p => (p.GetType() == typeof(Pushpin) && (string)((Pushpin)p).Tag == tag));
                // If a push pin was found
                if (pushpin != null)
                {
                    // Remove it, and go to the top
                    map1.Children.Remove(pushpin);
                    continue;
                }
                // If no push_pin was found
                else
                {
                    // Let's break the loop, because we removed them all.
                    break;
                }
            }
        }
        // Add a push-pin to the given location, with the given tag.
        public void set_pushpin(GeoCoordinate location, string tag = default_tag)
        {
            // Show our location with a pin on the map.
            Pushpin locationPushpin = new Pushpin();
            // Give it a name
            locationPushpin.Tag = tag;
            // And a location
            locationPushpin.Location = location;
            // And add it to our map
            map1.Children.Add(locationPushpin);
        }

        // Have the map, focus on a specific push_pin location.
        public void focus_pushpin(GeoCoordinate location)
        {
            // Focus our view on the pin
            map1.SetView(location, ri.checkZoom());
        }

        // Deactives a button, by fading and disabling it,
        //       or active it, by unfading and enabling it.
        public void active_fade_button(ButtonBase b, bool enabled)
        {
            if (enabled)
            {
                // Fade
                b.Opacity = 1;
            }
            else
            {
                // Unfade
                b.Opacity = 0.5;
            }
            // Enable / Unenable
            b.IsEnabled = enabled;
        }

        // Center the map, on the specified latitude & longtitude
        // TODO: Merge with `focus_pushpin`.
        public void center_map(double latitude, double longtitude)
        {
            map1.Center = new GeoCoordinate(latitude, longtitude);
        }

        // To be called when our app starts
        private void init()
        {
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
                    // Do the actual unpausing
                    ri.unpause();
                    // Updates the app with appropiate information
                    pauseButton.Content = "Pause";
                    infoBlock.Text += "Run resumed\n";
                }
                else
                {
                    // Greys out the unneeded and unreachable buttons
                    active_fade_button(stopButton, false);
                    // Do the actual pausing
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
                // Stop the watcher
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
            // Go to the load page
            NavigationService.Navigate(new Uri("/LoadPage.xaml", UriKind.Relative));
        }

        /* Handle the information given when the upload button is pressed
         * Currently tries to upload the current route to the cloudserver */
        private void uploadButton_Click(object sender, RoutedEventArgs e)
        {
            // Go to the save page
            NavigationService.Navigate(new Uri("/SavePage.xaml", UriKind.Relative));
        }

        public void changeTextInInfoBlock(string s1)
        {
            // Update the textblock
            infoBlock.Text = s1;
        }
    }
}
