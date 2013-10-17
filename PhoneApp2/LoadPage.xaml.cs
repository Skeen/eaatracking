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
using System.Collections.ObjectModel;
using System.Device.Location;

namespace PhoneApp2
{
    public partial class LoadPage : PhoneApplicationPage
    {
        private ObservableCollection<String> obs;

        public LoadPage()
        {
            InitializeComponent();
            OutputToServer.listRoutes(callback1);
        }

        public void callback1(List<string> routeIDs)
        {
            // We need to run the assignment of the observable colletion to the datacontext,
            // on the UI thread, hence this wierd dispatch code.
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                obs = new ObservableCollection<string>(routeIDs);
                lstbx1.DataContext = obs;
            });
        }        

        public void callback2(List<PositionInformation> wayPoints)
        {
            const string tag = "loaded_pushpins";
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                MainPage.getSingleton().clear_pushpins(tag);
                foreach (PositionInformation pi in wayPoints)
                {
                    GeoCoordinate location = new GeoCoordinate(pi.latitude, pi.longitude);
                    DateTimeOffset timestamp = pi.timeStamp;

                    MainPage.getSingleton().set_pushpin(location, timestamp, tag);
                }
            });
        }

        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void load_button_Click(object sender, RoutedEventArgs e)
        {
            if (lstbx1.SelectedIndex != -1)
            {
                String routeIDString = lstbx1.SelectedItem.ToString();
                routeIDString = routeIDString.Substring(0, routeIDString.Length - 4);
                System.Diagnostics.Debug.WriteLine(routeIDString);
                OutputToServer.listWaypoints(routeIDString, callback2);
                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Please select a route!");
            } 
        }
    }
}