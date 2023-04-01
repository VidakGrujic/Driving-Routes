using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivingRoutes.Model
{
    public class Point
    {
        private double longitude;
        private double latitude;

        public Point()
        {
            longitude = 0.0;
            latitude = 0.0;
        }

        public Point(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public double Longitude { get => longitude; set => longitude = value; }
        public double Latitude { get => latitude; set => latitude = value; }
    }
}
