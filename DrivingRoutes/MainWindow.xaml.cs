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
        ElementLoader elementLoader;
        Dictionary<long, Semaphore> semaphores;
        Dictionary<long, Roundabout> roundabouts;
        Dictionary<long, Path> paths;
        Dictionary<long, Element> allElements;
        string semaphoreOverlayId = "Semaphores";
        string roundaboutOverlayId = "Roundabouts";
        int scaleConstant = 20; //constant that will be used for the increasing the size of the marker
        string routeFilePath = "routes.xml";

        List<GMapMarker> routeMarkers; //this list will be used for marker that have been chosen for the route

        List<Route> routes;



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
            allElements = elementLoader.GetAllElements();

            paths = elementLoader.LoadPaths();
            routeMarkers = new List<GMapMarker>();

            routes = RouteCRUD.LoadRoutesFromFile();

        }

        private GMapOverlay GetSemaphoreOverlay(Dictionary<long, Semaphore> semaphores)
        {
            GMapOverlay semaphoresOverlay = new GMapOverlay(semaphoreOverlayId);
            foreach(Semaphore semaphore in semaphores.Values)
            {
                GMapMarker marker = new GMarkerGoogle(new PointLatLng(semaphore.Latitude, semaphore.Longitude), GMarkerGoogleType.yellow_dot);
                marker.ToolTipText = semaphore.ToString();
                semaphoresOverlay.Markers.Add(marker);
            }
            return semaphoresOverlay;
        }

        private GMapOverlay GetRoundaboutOverlay(Dictionary<long, Roundabout> roundabouts)
        {
            GMapOverlay roundaboutsOverlay = new GMapOverlay(roundaboutOverlayId);
            foreach(Roundabout roundabout in roundabouts.Values)
            {
                GMapMarker marker = new GMarkerGoogle(new PointLatLng(roundabout.Latitude, roundabout.Longitude), GMarkerGoogleType.blue_dot);
                marker.ToolTipText = roundabout.ToString();
                roundaboutsOverlay.Markers.Add(marker);
                
            }
            return roundaboutsOverlay;
        }

        private List<GMapRoute> GetPathRoutes(Dictionary<long, Path> paths)
        {
            List<GMapRoute> routePaths = new List<GMapRoute>();
            
            foreach(Path path in paths.Values)
            {
                GMapRoute routePath = new GMapRoute(HelperFunctions.ConvertPointsToLatLngPonts(path.Points), path.Id.ToString());
                routePath.Stroke = new System.Drawing.Pen(System.Drawing.Color.Black, 6);
                routePaths.Add(routePath);
            }
            return routePaths;
        }


        private void LoadModelButton_Click(object sender, RoutedEventArgs e)
        {
            gmap.Overlays.Add(GetSemaphoreOverlay(semaphores));
            gmap.Overlays.Add(GetRoundaboutOverlay(roundabouts));
            gmap.OnMarkerClick += new MarkerClick(marker_OnMarkerClick);

            gmap.Zoom++;
            gmap.Zoom--;

        }

        private void marker_OnMarkerClick(GMapMarker marker, MouseEventArgs e)
        {
            //left button click adds the marker into the list of chosen markers for the route
            if (e.Button == MouseButtons.Left)
            {
                marker.Size = new System.Drawing.Size(marker.Size.Width + scaleConstant, marker.Size.Height + scaleConstant);
                routeMarkers.Add(marker);
            }
            //right button click removes the marker from list of chosen markers fot the route
            else if(e.Button == MouseButtons.Right)
            {
                marker.Size = new System.Drawing.Size(marker.Size.Width - scaleConstant, marker.Size.Height - scaleConstant);
                routeMarkers.Remove(marker);
            }
        }

        private void AddRouteButton_Click(object sender, RoutedEventArgs e)
        {
            //ovo su mi svi elemnti koji se nalaze na tom putanju, tj. semafori i roundabouts.
            List<Element> routeElements = new List<Element>();

            //ovo su mi putanje koje se nalaze izmedju ovih elemenata
            List<Path> routePaths = new List<Path>(); 

            //there msut be at least 2 chosen markers for the route
            if (routeMarkers.Count <= 2)
            {
                errorLabel.Content = "It must be at least 2 chosen marker for the route";
                errorLabel.Foreground = Brushes.Red;
                errorLabel.BorderBrush = Brushes.Red;
            }
            else
            {
                errorLabel.Content = "";
                errorLabel.BorderBrush = Brushes.Black;


                //Step 1: Get list of markers IDs
                List<long> markerIDs = new List<long>();
                foreach(GMapMarker marker in routeMarkers)
                {
                    long id = GetMarkerId(marker);
                    markerIDs.Add(id);
                    routeElements.Add(allElements[id]);
                }
               

                //step 2: sort the list
                markerIDs = markerIDs.OrderBy(i => i).ToList();

                //step 3: get routes using this list
                for(int i = 0; i < markerIDs.Count; i++)
                {
                    /*
                    ne treba da pravim razlicite dictionary za svaki semafore i kruzne tokove
                    treba da spojim ta dva dictionary jer svakako imaju razlicite id
                    ako ih ne spojim, moracu za svaku glupost da proveravam da li je semafor ili kruzni tok sto nije dobro
                    */

                    //ovo je slucaj da je normalno
                    if(i != markerIDs.Count - 1)
                    {
                        long pathId = GetPathIdFromElements(markerIDs[i], markerIDs[i + 1]);
                        routePaths.Add(paths[pathId]);
                    }
                    else
                    {
                        //ovo slucaj da smo dosli do kraja i onda treba da spojimo prvi i poslednji element
                        long pathId = GetPathIdFromElements(markerIDs[0], markerIDs[markerIDs.Count - 1]);
                        routePaths.Add(paths[pathId]);
                    }
                }

                List<GMapRoute> routesPath = GetPathRoutes(routePaths.ToDictionary(p => p.Id, p => p));
                GMapOverlay routesOverlay = new GMapOverlay("Route");

                foreach(GMapRoute route in routesPath)
                {
                    routesOverlay.Routes.Add(route);
                }

                gmap.Overlays.Add(routesOverlay);

                gmap.Zoom++;
                gmap.Zoom--;

                //smanjimo elemente
                foreach(GMapMarker routeMarker in routeMarkers)
                {
                    routeMarker.Size = new System.Drawing.Size(routeMarker.Size.Width - scaleConstant, routeMarker.Size.Height - scaleConstant);
                }

                //moramo da ocistimo ovo jer moze svasta da se desi
                routeMarkers.Clear();

                //sad treba da upisem u fajl 
                // 1 - routeElements - tu su kruzni tokovi i semafori
                // 2 - routePaths - tu su sve putanje izmedju ta semafora i kruznih tokova

                //override metode equal da bi proveravali da li je ta ruta vec tu, ali tek nakon implementacije
                //upisivanja u fajl i citanja iz fajla

                if (string.IsNullOrEmpty(routeNameTextBox.Text))
                {
                    routeNameTextBox.BorderBrush = Brushes.Red;
                    errorLabel.Content = "There must be the name of the route";
                    errorLabel.BorderBrush = Brushes.Red;

                }
                else
                {
                    routeNameTextBox.BorderBrush = Brushes.Black;
                    errorLabel.BorderBrush = Brushes.Black;
                    double routeLength = Math.Round(HelperFunctions.CalculateRouteLength(routePaths), 2);

                    errorLabel.Content = $"The length of {routeNameTextBox.Name} route is {routeLength}km.";

                    Route newRoute = new Route(routeNameTextBox.Text, routeLength, routeElements, routePaths);
                    RouteCRUD.AddRouteToFile(newRoute);

                    errorLabel.Content = "Successfully added new route";

                }





                /*
                 algoritam radi tako sto sortiram lisstu id markera, potom u dictionary trazim trenutni id i sledeci id
                spojim ta dva id da bih dobio id od patha, potom nadjem path u dictionary
                kad nadjem path napravim GMapRoute i onda iscrtavam
                za dobijanje GMapRouta mogu da koristim ovu GetGMapRoute */

                /*ako hoocu da izbacim overlay ruta samo kazem gmap.overlays.remove(routeOverlay)
                 nastavljam neki drugi put, sad sam umoran*/


            }
        }

        private long GetPathIdFromElements(long firstId, long secondId)
        {
            string pathIdString = firstId.ToString() + secondId.ToString();
            return long.Parse(pathIdString);
        }

        private long GetMarkerId(GMapMarker marker)
        {
            //"Id: 1, Name: Semaphore 1 -> [Id][1][Name][Semaphore 1]
            string[] markerData = marker.ToolTipText.Split(new string[] { ", ", ": " }, StringSplitOptions.None);
            return long.Parse(markerData[1]);
        }
    }
}
