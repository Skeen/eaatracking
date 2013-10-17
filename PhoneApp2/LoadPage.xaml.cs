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

namespace PhoneApp2
{
    public partial class LoadPage : PhoneApplicationPage
    {
        ObservableCollection<String> obs;

        public LoadPage()
        {
            InitializeComponent();
            OutputToServer.listRoutes(callback);
        }

        public void callback(List<string> routeIDs)
        {
            // We need to run the assignment of the observable colletion to the datacontext,
            // on the UI thread, hence this wierd dispatch code.
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                obs = new ObservableCollection<string>(routeIDs);
                lstbx1.DataContext = obs;
            });
        }

        private void back_clicked(object sender, RoutedEventArgs e)
        {
            this.Content = new MainPage();
        }
    }
}