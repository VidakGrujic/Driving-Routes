using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DrivingRoutes.Model
{
   
    public class Element
    {
        private long id;
        private string name;
        private double longitude;
        private double latitude;

        public Element()
        {
           
        }

        public Element(long id, string name, double longitude, double latitude)
        {
            Id = id;
            Name = name;
            Longitude = longitude;
            Latitude = latitude;
        }

        public long Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public double Longitude { get => longitude; set => longitude = value; }
        public double Latitude { get => latitude; set => latitude = value; }

        public override string ToString()
        {
            return $"Id: {id}, Name: {name}";
        }
    }
}
