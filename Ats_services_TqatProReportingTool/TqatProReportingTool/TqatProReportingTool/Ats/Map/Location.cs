using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ats.Map {
    public class Location {
        public double latitude {
            get;
            set;
        }

        public double longitude {
            get;
            set;
        }

        public Location(double latitude, double longitude) {
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }

    public class Geofence {

        List<Location> points = new List<Location>();

        public Geofence() {
        }
        public Geofence(List<Location> points) {
            this.points = points;
        }

        public void addPoint(Location location) {
            this.points.Add(location);
        }

        public List<Location> getPoints() {
            return this.points;
        }

        public string Name {
            get;
            set;
        }

        public int Count {
            get {
                return this.points.Count;
            }
        }
    }
}
