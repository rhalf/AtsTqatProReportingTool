
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ats.Session {
    class Tracker {
        public int id {
            get;
            set;
        }
        public string companyDatabaseName {
            get;
            set;
        }
        public string vehicleRegistration {
            get;
            set;
        }
        public string driverName {
            get;
            set;
        }
        public string ownerName {
            get;
            set;
        }
        public string vehicleModel {
            get;
            set;
        }
        public DateTime vehicleRegistrationExpiry {
            get;
            set;
        }

        public string simNumber {
            get;
            set;
        }
        public string simImei {
            get;
            set;
        }
        public string deviceImei{
            get;
            set;
        }
     
        public int mobileDataProvider {
            get;
            set;
        }
        public int deviceType {
            get;
            set;
        }
        public string devicePassword {
            get;
            set;
        }
        public int imageNumber {
            get;
            set;
        }
        public int databaseHost {
            get;
            set;
        }
        public int httpHost {
            get;
            set;
        }
        public string dataDatabaseName {
            get;
            set;
        }
        public string users {
            get;
            set;
        }
        public string collections {
            get;
            set;
        }

        public DateTime dateCreated{
            get;
            set;
        }
        public DateTime dateExpired {
            get;
            set;
        }
        public string emails {
            get;
            set;
        }

        public int mileageInitial {
            get;
            set;
        }

        public int mileageLimit {
            get;
            set;
        }
        public int speedLimit {
            get;
            set;
        }

        public string inputs {
            get;
            set;
        }

        public int idlingTime {
            get;
            set;
        }

        public string note{
            get;
            set;
        }

    }
}
