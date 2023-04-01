using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivingRoutes.Model
{
    public class Path
    {
        private long id;
        private string name;
        private List<Point> points;

        public Path()
        {
            this.Points = new List<Point>();
        }

        public Path(long id, string name, List<Point> points)
        {
            this.Id = id;
            this.Name = name;
            this.Points = points;
        }

        public long Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public List<Point> Points { get => points; set => points = value; }
    }
}
