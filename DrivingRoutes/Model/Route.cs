using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivingRoutes.Model
{
    public class Route
    {
        private string routeName;
        private double routeLength;
        private List<Element> routeElements;
        private List<Path> routePaths;

        public Route()
        {

        }

        public Route(string routeName, double routeLength, List<Element> routeElements, List<Path> routePaths)
        {
            RouteName = routeName;
            RouteLength = routeLength;
            RouteElements = routeElements;
            RoutePaths = routePaths;
        }
        


        public string RouteName { get => routeName; set => routeName = value; }
        public List<Element> RouteElements { get => routeElements; set => routeElements = value; }
        public List<Path> RoutePaths { get => routePaths; set => routePaths = value; }
        public double RouteLength { get => routeLength; set => routeLength = value; }
    }
}
