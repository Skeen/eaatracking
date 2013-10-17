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

namespace PhoneApp2
{
    public partial class SavePage : PhoneApplicationPage
    {
        private List<PositionInformation> positions; 

        public SavePage(List<PositionInformation> init_positions)
        {
            positions = init_positions;
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new MainPage();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string route_name = textBox1.Text;
            foreach (PositionInformation pi in positions)
            {
                OutputToServer.sendData(route_name, pi);
            }
        }
    }
}