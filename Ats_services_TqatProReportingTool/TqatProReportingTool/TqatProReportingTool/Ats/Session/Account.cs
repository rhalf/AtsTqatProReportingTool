using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ats.Map;

namespace Ats.Session {

   public class Account {

       public List<Geofence> geofenceCollection = new List<Geofence>();
     

        public Account() {
        }

        public string companyUsername {
            get;
            set;
        }

        public string companyDisplayName {
            get;
            set;
        }

        public string username {
            get;
            set;
        }
        public string password {
            get;
            set;
        }
        public int accessLevel {
            get;
            set;
        }

        public int active {
            get;
            set;
        }

        public int id {
            get;
            set;
        }

        public DateTime dateTimeExpired{
            get;
            set;
        }

        public DateTime dateTimeCreated {
            get;
            set;
        }

        public string databaseName {
            get;
            set;
        }

        public bool rememberMe{
            get;
            set;
        }

    }
}
