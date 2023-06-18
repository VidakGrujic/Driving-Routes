using DrivingRoutes.Model;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = DrivingRoutes.Model.Path;
using Point = DrivingRoutes.Model.Point;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using MouseButtons = System.Windows.Forms.MouseButtons;
using System.Xml.Serialization;
using System.IO;

namespace DrivingRoutes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void SetActiveUserControl(UserControl control)
        {
            addControl.Visibility = Visibility.Collapsed;
            findControl.Visibility = Visibility.Collapsed;
            deleteControl.Visibility = Visibility.Collapsed;
            realTimeControl.Visibility = Visibility.Collapsed;
            control.Visibility = Visibility.Visible;
        }


        private void btnAddRoute_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(addControl);
        }

        private void btnFindRoute_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(findControl);
        }

        private void btnDeleteRoute_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(deleteControl);
        }

        private void btnRealTime_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(realTimeControl);
        }
    }
}
