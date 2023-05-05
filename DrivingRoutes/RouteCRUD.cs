using DrivingRoutes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DrivingRoutes
{
    public class RouteCRUD
    {

        public static void AddRouteToFile(Route route)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("routes.xml");

            //selektuijemo korenski 
            XmlNode routesNodes = doc.SelectSingleNode("Routes");

            XmlNode routeNode = doc.CreateElement("Route"); //kreiramo route element (ne zaboravi posle append na routesNodes)

            XmlNode routeName = doc.CreateElement("Name"); //kreiramo name element             
            routeName.InnerText = route.RouteName; //ubacujemo vrednost tog teksta u element
            routeNode.AppendChild(routeName); //ubacujemo element u roditeljski

            XmlNode routeLength = doc.CreateElement("Length"); //kreiramo length element
            routeLength.InnerText = route.RouteLength.ToString(); //ubacujemo vrednost duzine u element
            routeNode.AppendChild(routeLength); //ubacujemo element u roditeljski

            //kreiramo elemente, ovo ce biti lista tih elemenata
            XmlNode routeElements = doc.CreateElement("Elements");

            //potom prolazimo kroz svaki element i kreiramo element po element
            foreach (Element element in route.RouteElements)
            {
                //kreiramo jedan element
                XmlNode routeElement = doc.CreateElement("Element");

                //kreiramo node koji ce biti Id i njega ubacujemo u element
                XmlNode routeElementId = doc.CreateElement("Id");
                routeElementId.InnerText = element.Id.ToString();
                routeElement.AppendChild(routeElementId);

                //kreiramo node koji ce biti Name rute i ubacujemo je
                XmlNode routeElementName = doc.CreateElement("Name");
                routeElementName.InnerText = element.Name;
                routeElement.AppendChild(routeElementName);

                //kreiramo type (semafor ili roundabout ili nesto trece)
                XmlNode routeElementType = doc.CreateElement("Type");

                //moramo da splitujemo jer GetType() daje DrivingRoutes.Model.Semaphore
                //pa da bi izvukli samo semaphore onda uzmemo 2 element iz tog niza
                string[] temp = element.GetType().ToString().Split('.');
                routeElementType.InnerText = temp[2];
                routeElement.AppendChild(routeElementType);

                //kreiramo node za longitude 
                XmlNode routeElementLongitude = doc.CreateElement("Longitude");
                routeElementLongitude.InnerText = element.Longitude.ToString();
                routeElement.AppendChild(routeElementLongitude);

                //kreiramo node za latitude
                XmlNode routeElementLatitude = doc.CreateElement("Latitude");
                routeElementLatitude.InnerText = element.Latitude.ToString();
                routeElement.AppendChild(routeElementLatitude);

                //i na kraju tal element zalepimo u listu elemenata
                routeElements.AppendChild(routeElement);
            }

            //dodamo elemente u route node
            routeNode.AppendChild(routeElements);


            //sad idemo na paths
            XmlNode routePaths = doc.CreateElement("Paths");

            //prolazimo kroz sve paths i upisujemo ih
            foreach (Path path in route.RoutePaths)
            {
                //kreiramo jedan path element
                XmlNode routePath = doc.CreateElement("Path");

                //kreiramo nod za Id path
                XmlNode routePathId = doc.CreateElement("Id");
                routePathId.InnerText = path.Id.ToString();
                routePath.AppendChild(routePathId);

                //kreiramo za name
                XmlNode routePathName = doc.CreateElement("Name");
                routePathName.InnerText = path.Name;
                routePath.AppendChild(routePathName);

                //e sad posto u path imamo listu pointova, moracemo da napravimo node
                //za tu listu i onda potom za svaki point posebno sve

                //prvo pravimo listu pointova
                XmlNode routePoints = doc.CreateElement("Points");

                foreach (Point point in path.Points)
                {
                    //kreiramo jedan point
                    XmlNode routePoint = doc.CreateElement("Point");

                    //node za longitude
                    XmlNode routePointLongitude = doc.CreateElement("Longitude");
                    routePointLongitude.InnerText = point.Longitude.ToString();
                    routePoint.AppendChild(routePointLongitude);

                    //node za latitude
                    XmlNode routePointLantitude = doc.CreateElement("Latitude");
                    routePointLantitude.InnerText = point.Latitude.ToString();
                    routePoint.AppendChild(routePointLantitude);

                    //dodamo taj point u listu
                    routePoints.AppendChild(routePoint);
                }

                //dodamo listu pointova u nas path
                routePath.AppendChild(routePoints);

                //i na kraju path dodamo u paths 
                routePaths.AppendChild(routePath);
            }

            //dodamo pathove u route node
            routeNode.AppendChild(routePaths);

            //na kraju taj route node dodamo u routes
            routesNodes.AppendChild(routeNode);

            //sacuvamo to sto smo promenili
            doc.Save("routes.xml");




        }

        public static List<Route> LoadRoutesFromFile()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("routes.xml");

            XmlNodeList routesNodeList = doc.DocumentElement.SelectNodes("/Routes/Route");

            List<Route> routes = new List<Route>();

            foreach (XmlNode routeNode in routesNodeList)
            {
                Route route = new Route();
                route.RouteName = routeNode.SelectSingleNode("Name").InnerText;
                route.RouteLength = double.Parse(routeNode.SelectSingleNode("Length").InnerText);

                route.RouteElements = new List<Element>();
                route.RoutePaths = new List<Path>();

                //u ChildNodes[2] su elementi a ChildNodes[3] su paths
                foreach (XmlNode routeElement in routeNode.ChildNodes[2].ChildNodes)
                {
                    string type = routeElement.SelectSingleNode("Type").InnerText;
                    Element element = MakeElementObject(type);
                    element.Id = long.Parse(routeElement.SelectSingleNode("Id").InnerText);
                    element.Name = routeElement.SelectSingleNode("Name").InnerText;
                    element.Latitude = double.Parse(routeElement.SelectSingleNode("Latitude").InnerText);
                    element.Longitude = double.Parse(routeElement.SelectSingleNode("Longitude").InnerText);
                    route.RouteElements.Add(element);
                }

                foreach (XmlNode routePath in routeNode.ChildNodes[3].ChildNodes)
                {
                    Path path = new Path();
                    path.Id = long.Parse(routePath.SelectSingleNode("Id").InnerText);
                    path.Name = routePath.SelectSingleNode("Name").InnerText;
                    path.Points = new List<Point>();

                    //sad se lista points-ova nalazi na 3 mestu
                    foreach(XmlNode routePathPoint in routePath.ChildNodes[2].ChildNodes)
                    {
                        Point point = new Point();
                        point.Longitude = double.Parse(routePathPoint.SelectSingleNode("Longitude").InnerText);
                        point.Latitude = double.Parse(routePathPoint.SelectSingleNode("Latitude").InnerText);
                        path.Points.Add(point);
                    }

                    route.RoutePaths.Add(path);
                }

                routes.Add(route);

            }
            return routes;
        }

        private static Element MakeElementObject(string type)
        {
            switch (type)
            {
                case "Semaphore":
                    return new Semaphore();
                case "Roundabout":
                    return new Roundabout();
                default:
                    return null;
            }
        }



    }
}
