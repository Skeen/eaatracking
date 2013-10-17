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
            obs = new ObservableCollection<string>(OutputToServer.listRoutes());
            lstbx1.DataContext = obs;
        }

        private void back_clicked(object sender, RoutedEventArgs e)
        {
            this.Content = new MainPage();
        }
    }
}