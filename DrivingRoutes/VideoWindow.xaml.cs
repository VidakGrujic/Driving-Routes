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
using System.Windows.Threading;

namespace DrivingRoutes
{
    /// <summary>
    /// Interaction logic for VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow : Window
    {

        Dictionary<double, PointLatLng> videoPoints;
        GMarkerGoogle currentLocation;
        Route videoRoute;
        string videoRouteName = "Video Route";

        //markeri i pathovi video rute koje treba da se prikazu
      
      
        public VideoWindow(GMapControl gmap)
        {
            InitializeComponent();

            ElementLoader elementLoader = new ElementLoader();
            videoPoints = elementLoader.GetVideoPoints();

            videoRoute = RouteCRUD.GetRouteOnVideo(videoRouteName);
            gmap.Overlays.Add(RouteCRUD.GetVideoRouteOverlay(videoRoute));

            GMapOverlay overlay = new GMapOverlay("VideoPoints");
            currentLocation = new GMarkerGoogle(videoPoints[0], GMarkerGoogleType.red_pushpin);
            overlay.Markers.Add(currentLocation);
            gmap.Overlays.Add(overlay);

            gmap.Zoom++;
            gmap.Zoom--;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            mePlayer.Play();
            mePlayer.Stop();
            mePlayer.Volume = 0;

           
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if(mePlayer.Source != null)
            {
                if (mePlayer.NaturalDuration.HasTimeSpan)
                {
                    //u positionu je trenutno vreme, na osnovu njega pristupam recniku
                    
                    //prikazujem prvo vreme, pa onda lokaciju
                   
                    double currentSecond = Math.Floor(mePlayer.Position.TotalSeconds);
                    if (videoPoints.ContainsKey(currentSecond) && currentSecond != 0)
                    {
                        currentLocation.Position = videoPoints[currentSecond];
                    }

                    lblStatus.Content = String.Format(
                        "\t\t{0} / {1}" +
                        "\n\nRoute name: {2}" +
                        "\nCurrent location: {3} E, {4} N",
                             mePlayer.Position.ToString(@"mm\:ss"),
                             mePlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"),
                             videoRoute.RouteName,
                             currentLocation.Position.Lat,
                             currentLocation.Position.Lng
                         );


                }
                else
                {
                    lblStatus.Content = "No file selected...";
                }
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Play();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Pause();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Stop();
        }
    }
}
