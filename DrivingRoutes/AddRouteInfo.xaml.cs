using DrivingRoutes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DrivingRoutes
{
    /// <summary>
    /// Interaction logic for AddRouteInfo.xaml
    /// </summary>
    public partial class AddRouteInfo : Window
    {
        public AddRouteInfo()
        {
            InitializeComponent();
        }

        public AddRouteInfo(Route route)
        {
            InitializeComponent();

            nameTextBlock.Text = " " + route.RouteName;

            lengthTextBlock.Text = " " + route.RouteLength.ToString() + "km";

            foreach(Element e in route.RouteElements)
            {
                string s = "------------ Element ------------\n" +
                           $"Element ID: {e.Id}\n" +
                           $"Element Name: {e.Name}\n" +
                           $"Element Type: {e.GetType().ToString().Split('.')[2]}\n" +
                           "----------------------------------\n\n\n";
                routeElementsTextBox.Text += s;
                     
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
