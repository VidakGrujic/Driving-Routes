using DrivingRoutes.Model;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace DrivingRoutes
{
    public class ElementLoader
    {
        /// <summary>
        /// The file paths to file and elements in file
        /// </summary>
        private string filePath = "DrivingElements.xml";
        private string videoPointsFilePath = "VideoPositions.xml";
        private string semaphoresPath = "/DrivingElements/Semaphores/Semaphore";
        private string roundaboutsPath = "/DrivingElements/Roundabouts/Roundabout";
        private string pathsPath = "/DrivingElements/Paths/Path";
        private string videoPointsPaths = "/Positions/Position";
        private Dictionary<long, Element> allElements = new Dictionary<long, Element>();


        private XmlDocument xmlDoc;
        private XmlDocument xmlDocVideoPoints;
        public ElementLoader()
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            xmlDocVideoPoints = new XmlDocument();
            xmlDocVideoPoints.Load(videoPointsFilePath);
        }

        /// <summary>
        /// Loading the semaphores from file
        /// </summary>
        /// <param name="semaphores"></param>
        public Dictionary<long, Semaphore> LoadSemaphores()
        {
            Dictionary<long, Semaphore> semaphores = new Dictionary<long, Semaphore>();
            XmlNodeList semaphoreNodeList = xmlDoc.DocumentElement.SelectNodes(semaphoresPath);

            foreach (XmlNode semaphoreNode in semaphoreNodeList)
            {
                Semaphore semaphore = new Semaphore();
                semaphore.Id = long.Parse(semaphoreNode.SelectSingleNode("Id").InnerText);
                semaphore.Name = semaphoreNode.SelectSingleNode("Name").InnerText;
                semaphore.Longitude = double.Parse(semaphoreNode.SelectSingleNode("Long").InnerText);
                semaphore.Latitude = double.Parse(semaphoreNode.SelectSingleNode("Lat").InnerText);

                semaphores.Add(semaphore.Id, semaphore);
                allElements.Add(semaphore.Id, semaphore);

            }
            return semaphores;
        }

        /// <summary>
        /// Loading the roundabouts from file
        /// </summary>
        /// <param name="roundabouts"></param>
        public Dictionary<long, Roundabout> LoadRoundabouts()
        {
            Dictionary<long, Roundabout> roundabouts = new Dictionary<long, Roundabout>();
            XmlNodeList roundaboutNodeList = xmlDoc.DocumentElement.SelectNodes(roundaboutsPath);

            foreach (XmlNode roundaboutNode in roundaboutNodeList)
            {
                Roundabout roundabout = new Roundabout();
                roundabout.Id = long.Parse(roundaboutNode.SelectSingleNode("Id").InnerText);
                roundabout.Name = roundaboutNode.SelectSingleNode("Name").InnerText;
                roundabout.Longitude = double.Parse(roundaboutNode.SelectSingleNode("Long").InnerText);
                roundabout.Latitude = double.Parse(roundaboutNode.SelectSingleNode("Lat").InnerText);

                roundabouts.Add(roundabout.Id, roundabout);
                allElements.Add(roundabout.Id, roundabout);
            }
            return roundabouts;
        }

        public Dictionary<double, PointLatLng> GetVideoPoints()
        {
            Dictionary<double, PointLatLng> videoPoints = new Dictionary<double, PointLatLng>();
            XmlNodeList videoPointsNodeList = xmlDocVideoPoints.DocumentElement.SelectNodes(videoPointsPaths);

            foreach(XmlNode videoPointNode in videoPointsNodeList)
            {
                TimeSpan time = TimeSpan.Parse(videoPointNode.SelectSingleNode("Time").InnerText);
                PointLatLng point = new PointLatLng(double.Parse(videoPointNode.SelectSingleNode("Lat").InnerText),
                                                    double.Parse(videoPointNode.SelectSingleNode("Long").InnerText));
                videoPoints.Add(time.TotalSeconds, point);
            }
            return videoPoints;
        }
        


        public Dictionary<long, Path> LoadPaths()
        {
            Dictionary<long, Path> paths = new Dictionary<long, Path>();
            XmlNodeList pathNodeList = xmlDoc.DocumentElement.SelectNodes(pathsPath);
            
            foreach(XmlNode pathNode in pathNodeList)
            {
                Path path = new Path();
                path.Id = long.Parse(pathNode.SelectSingleNode("Id").InnerText);
                path.Name = pathNode.SelectSingleNode("Name").InnerText;
                path.Points = GetPathPoints(pathNode.SelectSingleNode("Points").InnerText);

                paths.Add(path.Id, path);
            }
            return paths;
        }
        private List<Point> GetPathPoints(string points)
        {
            List<Point> pointsList = new List<Point>();
            string[] pointsArray = points.Split(new string[] { "," }, StringSplitOptions.None);
            for(int i = 0; i < pointsArray.Length; i += 2)
            {
                /// example: 19.83024148770287,45.26406943662921,19.8392547517112,45.24794632549235
                /// first point long: 19.8302 it is pointsArray[0] 
                /// first point lat: 45.2640 it is pointsArray[0+1];
                /// i = i + 2 = 0 + 2 => i = 2
                /// second point long: 19.8392 it is pointsArray[2];
                /// second point lat: 45.247 it is pointsArray[2 + 1 = 3];                
                double longitude = double.Parse(pointsArray[i]);
                double latitude = double.Parse(pointsArray[i + 1]);
                Point point = new Point(longitude, latitude);
                pointsList.Add(point);
            }
            return pointsList;
        }


        public Dictionary<long, Element> GetAllElements()
        {
            return allElements;
        }

        
    }
}
