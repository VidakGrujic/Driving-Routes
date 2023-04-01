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
        private string filePath = "../../DrivingElements.xml";
        private string semaphoresPath = "/DrivingElements/Semaphores/Semaphore";
        private string roundaboutsPath = "/DrivingElements/Roundabouts/Roundabout";

        private XmlDocument xmlDoc;
        public ElementLoader()
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
        }



        /// <summary>
        /// Loading the semaphores from file
        /// </summary>
        /// <param name="semaphores"></param>
        public GMapOverlay LoadSemaphores(out Dictionary<long, Semaphore> semaphores)
        {
            semaphores = new Dictionary<long, Semaphore>();
            XmlNodeList semaphoreNodeList = xmlDoc.DocumentElement.SelectNodes(semaphoresPath);
            GMapOverlay semaphoresOverlay = new GMapOverlay("Semaphores");
            Semaphore semaphore = new Semaphore();

            foreach(XmlNode semaphoreNode in semaphoreNodeList)
            {
                semaphore.Id = long.Parse(semaphoreNode.SelectSingleNode("Id").InnerText);
                semaphore.Name = semaphoreNode.SelectSingleNode("Name").InnerText;
                semaphore.Longitude = double.Parse(semaphoreNode.SelectSingleNode("Long").InnerText);
                semaphore.Latitude = double.Parse(semaphoreNode.SelectSingleNode("Lat").InnerText);
                
                semaphores.Add(semaphore.Id, semaphore);
                GMapMarker marker = new GMarkerGoogle(new PointLatLng(semaphore.Latitude, semaphore.Longitude),  GMarkerGoogleType.yellow_dot);
                marker.ToolTipText = semaphore.ToString();
                semaphoresOverlay.Markers.Add(marker);
            }
            return semaphoresOverlay;
        }

        /// <summary>
        /// Loading the roundabouts from file
        /// </summary>
        /// <param name="roundabouts"></param>
        public GMapOverlay LoadRoundabouts(out Dictionary<long, Roundabout> roundabouts)
        {
            roundabouts = new Dictionary<long, Roundabout>();
            XmlNodeList roundaboutNodeList = xmlDoc.DocumentElement.SelectNodes(roundaboutsPath);
            GMapOverlay roundaboutOverlay = new GMapOverlay("Roundabout");
            Roundabout roundabout = new Roundabout();
            
            foreach(XmlNode roundaboutNode in roundaboutNodeList)
            {
                roundabout.Id = long.Parse(roundaboutNode.SelectSingleNode("Id").InnerText);
                roundabout.Name = roundaboutNode.SelectSingleNode("Name").InnerText;
                roundabout.Longitude = double.Parse(roundaboutNode.SelectSingleNode("Long").InnerText);
                roundabout.Latitude = double.Parse(roundaboutNode.SelectSingleNode("Lat").InnerText);

                roundabouts.Add(roundabout.Id, roundabout);
                GMapMarker marker = new GMarkerGoogle(new PointLatLng(roundabout.Latitude, roundabout.Longitude), GMarkerGoogleType.blue_dot);
                marker.ToolTipText = roundabout.ToString();
                roundaboutOverlay.Markers.Add(marker);
            }
            return roundaboutOverlay;
        }

        
       
        






    }
}
