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
    /// Interaction logic for DeleteRoute.xaml
    /// </summary>
    public partial class DeleteRoute : UserControl
    {
        //ovde cuvam sve rute kao klasu Route
        Dictionary<string, Route> routes;

        //ovde za jednu rutu pravim listu markera koji predstavljaju element
        Dictionary<string, List<GMapMarker>> routesElementMarkers;

        //ovde za jednu rutu pravim listu putanja koja ce se iscrtati na mapi
        Dictionary<string, List<GMapRoute>> routesPathMarkers;

        //ovo cuvamo za retrieve
        Route routeRetrieve;
        List<GMapMarker> routeElementMarkersRetrieve = new List<GMapMarker>();
        List<GMapRoute> routePathMarkersRetrieve = new List<GMapRoute>();


        public DeleteRoute()
        {
            InitializeComponent();

            DataContext = this.DataContext;

            GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
            GMapProvider.WebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            gmap.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            //gmap.SetPositionByKeywords("Novi Sad, Serbia");
            gmap.Position = new GMap.NET.PointLatLng(45.254765, 19.844184);
            gmap.ShowCenter = false;
        }

        private void LoadRoutesButton_Click(object sender, RoutedEventArgs e)
        {
            routes = RouteCRUD.LoadRoutesFromFile().ToDictionary(p => p.RouteName, p => p);

            //napravimo za svaku rutu po marker
            routesElementMarkers = GetRoutesElementsMarkers(routes);

            //sad za svaku rutu ucitamo path
            routesPathMarkers = GetRoutesPathsMarkers(routes);

            //postavimo item source na kolekciju kljuceva
            foreach(string key in routes.Keys)
            {
                routesComboBox.Items.Add(key);
            }


            routesComboBox.Text = routes.Keys.ToArray()[0];

            //kad jednom ucita model da ne moze opet
            LoadRoutesButton.IsEnabled = false;
        }

        private void deleteRouteButton_Click(object sender, RoutedEventArgs e)
        {
            string deleteRouteName = routesComboBox.SelectedItem.ToString();

            Route deleteRoute = routes[deleteRouteName];
            if (RouteCRUD.DeleteRouteFromFile(deleteRoute))
            {
                if(routesComboBox.SelectedIndex != -1)
                {
                    //cuvamo poslednje obrisan da bi mogli da ga vratimo
                    routeRetrieve = deleteRoute;
                    routeElementMarkersRetrieve = routesElementMarkers[deleteRouteName];
                    routePathMarkersRetrieve = routesPathMarkers[deleteRouteName];

                    routes.Remove(deleteRouteName);
                    routesElementMarkers.Remove(deleteRouteName);
                    routesPathMarkers.Remove(deleteRouteName);


                    //posto se iz nekog razloga poziva event, mi taj evenet obrisemo pa posle
                    //brisanja itema mi ga ponovo dodajemo
                    routesComboBox.SelectionChanged -= routesComboBox_SelectionChanged;

                    //provera u slucaju sve rute da obrisemo
                    if (routes.Keys.Count != 0)
                    {
                        routesComboBox.Items.Remove(deleteRouteName);
                        routesComboBox.Text = routes.Keys.ToArray()[0];
                    }
                    routesComboBox.SelectionChanged += routesComboBox_SelectionChanged;

                }
            }

        }

        private void retrieveRouteButton_Click(object sender, RoutedEventArgs e)
        {
           //sad radimo retrieve
           if(routeRetrieve != null)
           {
                //dodamo u fajl
                RouteCRUD.AddRouteToFile(routeRetrieve);

                //dodamo u dictionarie
                routes.Add(routeRetrieve.RouteName, routeRetrieve);
                routesElementMarkers.Add(routeRetrieve.RouteName, routeElementMarkersRetrieve);
                routesPathMarkers.Add(routeRetrieve.RouteName, routePathMarkersRetrieve);

                //posto se iz nekog razloga poziva event, mi taj evenet obrisemo pa posle
                //brisanja itema mi ga ponovo dodajemo
                routesComboBox.SelectionChanged -= routesComboBox_SelectionChanged;
                routesComboBox.Items.Add(routeRetrieve.RouteName);
                routesComboBox.Text = routes.Keys.ToArray()[0];
                routesComboBox.SelectionChanged += routesComboBox_SelectionChanged;

                //vratimo na null jer nema elemenata za retrieve
                routeRetrieve = null;
                routeElementMarkersRetrieve = null;
                routePathMarkersRetrieve = null;
           }

        }

        private void routesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ovo je kljuc
            string selectedRouteName = routesComboBox.SelectedItem.ToString();
            Route selectedRoute = routes[selectedRouteName];

            //sad tu rutu treba da prixazemo
            ShowRoute(selectedRouteName, selectedRoute);

        }

        private Dictionary<string, List<GMapMarker>> GetRoutesElementsMarkers(Dictionary<string, Route> routes)
        {
            Dictionary<string, List<GMapMarker>> routesElementsMarkers = new Dictionary<string, List<GMapMarker>>();
            foreach (KeyValuePair<string, Route> route in routes)
            {
                List<GMapMarker> elementMarkers = new List<GMapMarker>();
                foreach (Element element in route.Value.RouteElements)
                {
                    GMapMarker elementMarker = new GMarkerGoogle(new PointLatLng(element.Latitude, element.Longitude), GetMarkerType(element));
                    elementMarker.ToolTipText = element.ToString();
                    elementMarkers.Add(elementMarker);
                }
                routesElementsMarkers.Add(route.Key, elementMarkers);
            }
            return routesElementsMarkers;
        }

        private Dictionary<string, List<GMapRoute>> GetRoutesPathsMarkers(Dictionary<string, Route> routes)
        {
            Dictionary<string, List<GMapRoute>> routesPathsMarkers = new Dictionary<string, List<GMapRoute>>();
            foreach (KeyValuePair<string, Route> route in routes)
            {
                List<GMapRoute> pathRoute = new List<GMapRoute>();
                foreach (Path path in route.Value.RoutePaths)
                {
                    GMapRoute pathMarker = new GMapRoute(HelperFunctions.ConvertPointsToLatLngPonts(path.Points), path.Id.ToString());
                    pathMarker.Stroke = new System.Drawing.Pen(System.Drawing.Color.Black, 6);
                    pathRoute.Add(pathMarker);
                }
                routesPathsMarkers.Add(route.Key, pathRoute);
            }
            return routesPathsMarkers;
        }

        private GMarkerGoogleType GetMarkerType(Element element)
        {
            switch (element.GetType().ToString().Split('.')[2])
            {
                case "Semaphore":
                    return GMarkerGoogleType.yellow_dot;
                case "Roundabout":
                    return GMarkerGoogleType.blue_dot;
                default:
                    return GMarkerGoogleType.black_small;
            }
        }

        private void ShowRoute(string selectedRouteName, Route selectedRoute)
        {
            //ciscenje prethodne rute
            gmap.Overlays.Clear();
            routeElementsTextBox.Text = "";

            //dodavanje nove rute
            GMapOverlay routeOverlay = new GMapOverlay(selectedRouteName);
            foreach (GMapMarker marker in routesElementMarkers[selectedRouteName])
            {
                routeOverlay.Markers.Add(marker);
            }

            foreach (GMapRoute path in routesPathMarkers[selectedRouteName])
            {
                routeOverlay.Routes.Add(path);
            }

            gmap.Overlays.Add(routeOverlay);
            gmap.Zoom++;
            gmap.Zoom--;

            //ispisivsanje rute
            string s = "";
            s += $"Route Name: {selectedRoute.RouteName}\n" +
                 $"Route Length: {selectedRoute.RouteLength}km\n\n";

            foreach (Element el in selectedRoute.RouteElements)
            {
                s += "------------ Element ------------\n" +
                      $"Element ID: {el.Id}\n" +
                      $"Element Name: {el.Name}\n" +
                      $"Element Type: {el.GetType().ToString().Split('.')[2]}\n" +
                      "----------------------------------\n\n";
            }
            routeElementsTextBox.Text = s;
        }

    }
}
