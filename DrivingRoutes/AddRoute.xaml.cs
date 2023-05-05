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
    /// Interaction logic for AddRoute.xaml
    /// </summary>
    public partial class AddRoute : UserControl
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

        GMapOverlay routesOverlay; //ovo mi treba van jer hocu da obrisem staru rutu kad dodajem novu

        List<GMapMarker> routeMarkers; //this list will be used for marker that have been chosen for the route

        List<Route> routes;
        public AddRoute()
        {
            InitializeComponent();

            GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
            GMapProvider.WebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            gmap.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            //gmap.SetPositionByKeywords("Novi Sad, Serbia");
            gmap.Position = new GMap.NET.PointLatLng(45.254765, 19.844184);
            gmap.ShowCenter = false;

            routeMarkers = new List<GMapMarker>();

           
        }

        private GMapOverlay GetSemaphoreOverlay(Dictionary<long, Semaphore> semaphores)
        {
            GMapOverlay semaphoresOverlay = new GMapOverlay(semaphoreOverlayId);
            foreach (Semaphore semaphore in semaphores.Values)
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
            foreach (Roundabout roundabout in roundabouts.Values)
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

            foreach (Path path in paths.Values)
            {
                GMapRoute routePath = new GMapRoute(HelperFunctions.ConvertPointsToLatLngPonts(path.Points), path.Id.ToString());
                routePath.Stroke = new System.Drawing.Pen(System.Drawing.Color.Black, 6);
                routePaths.Add(routePath);
            }
            return routePaths;
        }

        private void LoadModelButton_Click(object sender, RoutedEventArgs e)
        {
            //moram ovde da ucitavam jer onda baca neki problem, ne smem u konstruktoru
            elementLoader = new ElementLoader();
            semaphores = elementLoader.LoadSemaphores();
            roundabouts = elementLoader.LoadRoundabouts();
            allElements = elementLoader.GetAllElements();
            paths = elementLoader.LoadPaths();

            routes = RouteCRUD.LoadRoutesFromFile();
 
            gmap.Overlays.Add(GetSemaphoreOverlay(semaphores));
            gmap.Overlays.Add(GetRoundaboutOverlay(roundabouts));
            gmap.OnMarkerClick += new MarkerClick(marker_OnMarkerClick);

            gmap.Zoom++;
            gmap.Zoom--;

        }

        private void marker_OnMarkerClick(GMapMarker marker, MouseEventArgs e)
        {
            //left button click adds the marker into the list of chosen markers for the route
            if (e.Button == MouseButtons.Left /*ovde dodati zastitu ako korisnik 2 puta klikne na element*/)
            {
                marker.Size = new System.Drawing.Size(marker.Size.Width + scaleConstant, marker.Size.Height + scaleConstant);
                routeMarkers.Add(marker);
            }
            //right button click removes the marker from list of chosen markers fot the route
            else if (e.Button == MouseButtons.Right)
            {
                marker.Size = new System.Drawing.Size(marker.Size.Width - scaleConstant, marker.Size.Height - scaleConstant);
                routeMarkers.Remove(marker);
            }
        }

        private void AddRouteButton_Click(object sender, RoutedEventArgs e)
        {
            //brisemo staru rutu
            gmap.Overlays.Remove(routesOverlay);
            
            //ovo su mi svi elemnti koji se nalaze na tom putanju, tj. semafori i roundabouts.
            List<Element> routeElements = new List<Element>();

            //ovo su mi putanje koje se nalaze izmedju ovih elemenata
            List<Path> routePaths = new List<Path>();

            //there msut be at least 2 chosen markers for the route
            if (routeMarkers.Count <= 2)
            {
                errorTextBlock.Text = "It must be at least 2 chosen marker for the route";
                errorTextBlock.Foreground = Brushes.Red;

                ReturnToDefaultSize(routeMarkers);
                
            }
            else if (string.IsNullOrEmpty(routeNameTextBox.Text))
            {
                routeNameTextBox.BorderBrush = Brushes.Red;
                errorTextBlock.Text = "There must be the name of the route";
                errorTextBlock.Foreground = Brushes.Red;

                //ovo ne treba da se vraca na staro jer korisnik samo treba da doda ime
                //ReturnToDefaultSize(routeMarkers);
            }
            else if (IsRouteNameExists(routeNameTextBox.Text))
            {
                routeNameTextBox.BorderBrush = Brushes.Red;
                errorTextBlock.Text = "There is already route with the same name";
                errorTextBlock.Foreground = Brushes.Red;

                //ovo ne treba da se vraca na staro jer korisnik samo treba da doda ime
                //ReturnToDefaultSize(routeMarkers);
            }
            else
            {
                errorTextBlock.Text = "";
                routeNameTextBox.BorderBrush = Brushes.Black;

                //Step 1: Get list of markers IDs
                List<long> markerIDs = new List<long>();
                foreach (GMapMarker marker in routeMarkers)
                {
                    long id = GetMarkerId(marker);
                    markerIDs.Add(id);
                    routeElements.Add(allElements[id]);
                }

                //ako ruta ne postoji onda je mozemo dodati
                if (!IsRouteExists(routeElements))
                {
                    //step 2: sort the list
                    markerIDs = markerIDs.OrderBy(i => i).ToList();

                    //step 3: get routes using this list
                    for (int i = 0; i < markerIDs.Count; i++)
                    {
                        /*
                        ne treba da pravim razlicite dictionary za svaki semafore i kruzne tokove
                        treba da spojim ta dva dictionary jer svakako imaju razlicite id
                        ako ih ne spojim, moracu za svaku glupost da proveravam da li je semafor ili kruzni tok sto nije dobro
                        */

                        //ovo je slucaj da je normalno
                        if (i != markerIDs.Count - 1)
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
                    routesOverlay = new GMapOverlay("Route");

                    foreach (GMapRoute route in routesPath)
                    {
                        routesOverlay.Routes.Add(route);
                    }

                    gmap.Overlays.Add(routesOverlay);

                    gmap.Zoom++;
                    gmap.Zoom--;

                    //smanjimo elemente
                    ReturnToDefaultSize(routeMarkers);


                    //sad treba da upisem u fajl 
                    // 1 - routeElements - tu su kruzni tokovi i semafori
                    // 2 - routePaths - tu su sve putanje izmedju ta semafora i kruznih tokova

                    //provere da li ta ista ruta vec postoji i da li njeno ime vec postoji

                    routeNameTextBox.BorderBrush = Brushes.Black;

                    double routeLength = Math.Round(HelperFunctions.CalculateRouteLength(routePaths), 2);

                    errorTextBlock.Text = $"The length of {routeNameTextBox.Name} route is {routeLength}km.";

                    Route newRoute = new Route(routeNameTextBox.Text, routeLength, routeElements, routePaths);
                    routes.Add(newRoute);
                    RouteCRUD.AddRouteToFile(newRoute);

                    errorTextBlock.Text = "Successfully added new route";
                    errorTextBlock.Foreground = Brushes.Green;

                    AddRouteInfo addRouteInfoWindow = new AddRouteInfo(newRoute);
                    addRouteInfoWindow.Show();


                    /*
                     algoritam radi tako sto sortiram lisstu id markera, potom u dictionary trazim trenutni id i sledeci id
                    spojim ta dva id da bih dobio id od patha, potom nadjem path u dictionary
                    kad nadjem path napravim GMapRoute i onda iscrtavam
                    za dobijanje GMapRouta mogu da koristim ovu GetGMapRoute 
                    */

                    /*ako hoocu da izbacim overlay ruta samo kazem gmap.overlays.remove(routeOverlay)
                     nastavljam neki drugi put, sad sam umoran*/

                }
                else
                {
                    errorTextBlock.Text = "Route exists, please make a new route";
                    errorTextBlock.Foreground = Brushes.Red;

                    ReturnToDefaultSize(routeMarkers);
                }
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

        private bool IsRouteNameExists(string newRouteName)
        {
            foreach (Route route in routes)
            {
                if (route.RouteName.Equals(newRouteName))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsRouteExists(List<Element> routeElements)
        {
            /*
             prvo naci route koje imaju isti broj elemenata 
            */
            List<Route> sameNumElementRoutes = new List<Route>();
            foreach (Route route in routes)
            {
                if (route.RouteElements.Count == routeElements.Count)
                {
                    sameNumElementRoutes.Add(route);
                }
            }

            /*
             Sad proci za svaku rutu kroz njene elemente i uporedjivati ih
             */
            foreach (Route route in sameNumElementRoutes)
            {
                int counter = 0;
                for (int i = 0; i < routeElements.Count; i++)
                {
                    //ako ne sadrzi element, onda nije ista ruta i izlazimo iz petlje
                    if (!route.RouteElements.Contains(routeElements[i]))
                    {
                        break;
                    }
                    else
                    {
                        //ako sadrzi, onda cemo pamtiti koliko elemenata sadrzi
                        counter++;
                    }
                }
                
                //kad izadje iz for petlje, ako je counter isti kao i broj elementa, onda je to ta ruta 
                //sto znaci da ta ruta vec postoji i ne moze da se doda
                if(counter == routeElements.Count)
                {
                    return true;
                }
            }
            return false;
        }

        private void ReturnToDefaultSize(List<GMapMarker> routeMarkers)
        {
            foreach (GMapMarker routeMarker in routeMarkers)
            {
                routeMarker.Size = new System.Drawing.Size(routeMarker.Size.Width - scaleConstant, routeMarker.Size.Height - scaleConstant);
            }
            routeMarkers.Clear();
            gmap.Zoom++;
            gmap.Zoom--;
        }
    }
}
