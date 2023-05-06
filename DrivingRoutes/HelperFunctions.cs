using DrivingRoutes.Model;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivingRoutes
{
    public class HelperFunctions
    {
        public static List<PointLatLng> ConvertPointsToLatLngPonts(List<Point> points)
        {
            List<PointLatLng> pathPoints = new List<PointLatLng>();
            foreach (Point point in points)
            {
                pathPoints.Add(new PointLatLng(point.Latitude, point.Longitude));
            }
            return pathPoints;
        }

        public static double CalculateRouteLength(List<Path> routePaths)
        {
            double routeLength = 0;
            foreach (Path path in routePaths)
            {
                routeLength += CalculatePathLength(path.Points);
            }
            return routeLength;
        }

        
        private static double CalculatePathLength(List<Point> points)
        {
            double pathLength = 0;
            /*
             Ako imamo 3 elementa, nama treba 2 iteracije. Distanca izmedju 0. i 1. elementa, pa izmedju 1. i 2. elementa i to je kraj
             */
            for(int i = 0; i < points.Count - 1; i++)
            {
                double lat1 = ConvertFromDecimalToRadius(points[i].Latitude);
                double lon1 = ConvertFromDecimalToRadius(points[i].Longitude);
                double lat2 = ConvertFromDecimalToRadius(points[i + 1].Latitude);
                double lon2 = ConvertFromDecimalToRadius(points[i + 1].Longitude);

                //acos(sin(lat1)*sin(lat2)+cos(lat1)*cos(lat2)*cos(lon2-lon1))*6371
                pathLength += Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2)) * 6371;
            }
            return pathLength;
        }

        private static double ConvertFromDecimalToRadius(double num)
        {
            return num / 180 * Math.PI; 
        }

       

    }
}
