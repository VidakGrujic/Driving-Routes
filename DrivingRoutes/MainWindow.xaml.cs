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

namespace DrivingRoutes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ElementLoader elementLoader;
        Dictionary<long, Semaphore> semaphores;
        Dictionary<long, Roundabout> roundabouts;
        public MainWindow()
        {
            InitializeComponent();

            GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
            GMapProvider.WebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            gmap.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            //gmap.SetPositionByKeywords("Novi Sad, Serbia");
            gmap.Position = new GMap.NET.PointLatLng(45.254765, 19.844184);
            gmap.ShowCenter = false;
            elementLoader = new ElementLoader();
            semaphores = elementLoader.LoadSemaphores();
            roundabouts = elementLoader.LoadRoundabouts();

        }

        private GMapOverlay AddSemaphoresToMap(Dictionary<long, Semaphore> semaphores)
        {
            GMapOverlay semaphoresOverlay = new GMapOverlay("Semaphores");
            foreach(Semaphore semaphore in semaphores.Values)
            {

                GMapMarker marker = new GMarkerGoogle(new PointLatLng(semaphore.Latitude, semaphore.Longitude), GMarkerGoogleType.yellow_dot);
                marker.ToolTipText = semaphore.ToString();
                //TODO: Here you should add event for mouse click, enetr and leave click

                semaphoresOverlay.Markers.Add(marker);
            }
            return semaphoresOverlay;
        }

        private GMapOverlay AddRoundaboutsToMap(Dictionary<long, Roundabout> roundabouts)
        {
            GMapOverlay roundaboutsOverlay = new GMapOverlay("Roundabouts");
            foreach(Roundabout roundabout in roundabouts.Values)
            {
                
                GMapMarker marker = new GMarkerGoogle(new PointLatLng(roundabout.Latitude, roundabout.Longitude), GMarkerGoogleType.blue_dot);
                marker.ToolTipText = roundabout.ToString();
                roundaboutsOverlay.Markers.Add(marker);
            }
            return roundaboutsOverlay;
        }

        private void LoadModelButton_Click(object sender, RoutedEventArgs e)
        {
            gmap.Overlays.Add(AddSemaphoresToMap(semaphores));
            gmap.Overlays.Add(AddRoundaboutsToMap(roundabouts));
               



            gmap.Zoom++;
            gmap.Zoom--;
        }
    }
}
