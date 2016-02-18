using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ats.Map {
    class MapTools {
        public bool IsPointInPolygon(Geofence geoFence, Location point) {
            List<Location> location =  geoFence.getPoints();

            int indexX, indexJ;
            bool status = false;
            for (indexX = 0, indexJ = location.Count - 1; indexX < location.Count; indexJ = indexX++) {
                if ((((location[indexX].latitude <= point.latitude) && (point.latitude < location[indexJ].latitude))
                        || ((location[indexJ].latitude <= point.latitude) && (point.latitude < location[indexX].latitude)))
                        && (point.longitude < (location[indexJ].longitude - location[indexX].longitude) * (point.latitude - location[indexX].latitude)
                            / (location[indexJ].latitude - location[indexX].latitude) + location[indexX].longitude)) {
                    status = !status;
                }
            }
            return status;
        }
        public bool IsPointInPolygon(List<Location> location, Location point) {

            int indexX, indexJ;
            bool status = false;
            for (indexX = 0, indexJ = location.Count - 1; indexX < location.Count; indexJ = indexX++) {
                if ((((location[indexX].latitude <= point.latitude) && (point.latitude < location[indexJ].latitude))
                        || ((location[indexJ].latitude <= point.latitude) && (point.latitude < location[indexX].latitude)))
                        && (point.longitude < (location[indexJ].longitude - location[indexX].longitude) * (point.latitude - location[indexX].latitude)
                            / (location[indexJ].latitude - location[indexX].latitude) + location[indexX].longitude)) {
                    status = !status;
                }
            }
            return status;
        }
    }

}


