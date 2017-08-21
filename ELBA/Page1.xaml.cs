using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Device.Location;
using Microsoft.Phone.Tasks;
namespace ELBA
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
        }
        private void home_click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Page4.xaml?", UriKind.RelativeOrAbsolute));
        }

        private void directions_click(object sender, EventArgs e)
        {
            MapsDirectionsTask mdt = new MapsDirectionsTask();
            LabeledMapLocation spd = new LabeledMapLocation(textBox1.Text,null);
            mdt.End = spd;
            mdt.Show();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Page2.xaml?", UriKind.RelativeOrAbsolute));
        }
    }
}