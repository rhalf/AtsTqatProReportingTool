using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ats.Map;

namespace Ats.Session {

    public class User {


        public int Id {
            get;
            set;
        }
        public string Username {
            get;
            set;
        }
        public string Password {
            get;
            set;
        }
        public string Email {
            get;
            set;
        }
        public string Main {
            get;
            set;
        }
        public string Timezone {
            get;
            set;
        }

        //master 1
        public int AccessLevel {
            get;
            set;
        }
        public DateTime DateTimeExpired {
            get;
            set;
        }
        public DateTime DateTimeCreated {
            get;
            set;
        }
        public bool IsActive {
            get;
            set;
        }
        public string DatabaseName {
            get;
            set;
        }
        public bool? RememberMe {
            get;
            set;
        }
        public List<Poi> Pois {
            get;
            set;
        }

        public List<Collection> Collections {
            get;
            set;
        }
    }
    
}
