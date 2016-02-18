using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ats.Session {
    public class Poi {
        public int Id {
            get;
            set;
        }

        public Coordinate Coordinate {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }
        public string Description {
            get;
            set;
        }

        public string Image {
            get;
            set;
        }
    }
}
