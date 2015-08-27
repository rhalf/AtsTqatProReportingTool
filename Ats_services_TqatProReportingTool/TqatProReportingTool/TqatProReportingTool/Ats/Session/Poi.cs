using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ats.Session {
    public class Poi {
        public int id {
            get;
            set;
        }

        public Coordinate location {
            get;
            set;
        }

        public string name {
            get;
            set;
        }
        public string description {
            get;
            set;
        }

        public string image {
            get;
            set;
        }
    }
}
