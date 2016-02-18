using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Globalization;

using MySql.Data.MySqlClient;

using TqatProReportingTool.Properties;
using Ats.Session;
using Ats.Helper;
using Ats.Devices.Meitrack;
using Ats.Parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ats.Database {



    class Query {

        MySqlConnection mysqlConnection = null;
        Database database;
        String sql;
        //User account;

        public Query(string sql, Database database) {
            if (database == null) {
                throw new QueryException(1, "Database is null.");
            }
            if (String.IsNullOrEmpty(sql)) {
                throw new QueryException(1, "Sql is null or empty.");
            }
            this.database = database;
            this.sql = sql;

            mysqlConnection = new MySqlConnection(this.database.getConnectionString());
        }

        public Query(Database database) {
            if (database == null) {
                throw new QueryException(1, "Database is null.");
            }

            this.database = database;

            mysqlConnection = new MySqlConnection(this.database.getConnectionString());
        }





        public DataTable getAllCompanies() {
            DataTable dataTable = this.getReportTable(ReportType.ALL_COMPANIES);
            try {
                mysqlConnection.Open();

                string sql =
                "SELECT * " +
                "FROM dbt_tracking_master.cmps;";

                MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);

                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                if (!mySqlDataReader.HasRows) {
                    throw new QueryException(1, "Companies's Collection is empty.");
                } else {
                    while (mySqlDataReader.Read()) {

                        DataRow dataRow = dataTable.NewRow();
                        dataRow["id"] = mySqlDataReader.GetInt32("cmpid");
                        dataRow["host"] = mySqlDataReader.GetString("cmphost");
                        dataRow["username"] = mySqlDataReader.GetString("cmpname");
                        dataRow["displayName"] = mySqlDataReader.GetString("cmpdisplayname");
                        dataRow["email"] = mySqlDataReader.GetString("cmpemail");
                        dataRow["address"] = mySqlDataReader.GetString("cmpaddress");
                        dataRow["telephoneNumber"] = mySqlDataReader.GetString("cmpphoneno");
                        dataRow["mobileNumber"] = mySqlDataReader.GetString("cmpmobileno");
                        dataRow["databaseName"] = mySqlDataReader.GetString("cmpdbname");
                        dataRow["isActive"] = (mySqlDataReader.GetString("cmpactive") == "1") ? true : false;

                        dataRow["dateCreated"] = Converter.subStandardDateTimeToDateTime(mySqlDataReader.GetString("cmpcreatedate"));
                        dataRow["dateExpired"] = Converter.subStandardDateTimeToDateTime(mySqlDataReader.GetString("cmpexpiredate"));

                        dataTable.Rows.Add(dataRow);
                    }
                    mySqlDataReader.Dispose();
                }

            } catch (QueryException queryException) {
                throw queryException;
            } catch (MySqlException mySqlException) {
                throw new QueryException(1, mySqlException.Message);
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {
                mysqlConnection.Close();
            }
            return dataTable;
        }

        public DataTable getAllTrackers() {
            DataTable dataTable = this.getReportTable(ReportType.ALL_TRACKERS);

            try {
                mysqlConnection.Open();

                string sql =
                    "SELECT * " +
                    "FROM dbt_tracking_master.trks;";

                MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);

                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                if (!mySqlDataReader.HasRows) {
                    throw new QueryException(1, "Tracker's Collection is empty.");
                } else {


                    Tracker tracker = new Tracker();
                    string dateTime;

                    while (mySqlDataReader.Read()) {

                        //tracker.Collections = (string)mySqlDataReader["tcollections"];
                        tracker.CompanyDatabaseName = (string)mySqlDataReader["tcmp"];
                        tracker.DatabaseHost = int.Parse((string)mySqlDataReader["tdbhost"]);
                        tracker.DatabaseName = (string)mySqlDataReader["tdbs"];

                        dateTime = (string)mySqlDataReader["tcreatedate"];
                        tracker.DateTimeCreated = Converter.subStandardDateTimeToDateTime(dateTime);
                        dateTime = String.Empty;

                        dateTime = (string)mySqlDataReader["ttrackerexpiry"];
                        tracker.DateTimeExpired = Converter.subStandardDateTimeToDateTime(dateTime);
                        dateTime = String.Empty;

                        tracker.TrackerImei = (string)mySqlDataReader["tunit"];
                        tracker.DevicePassword = (string)mySqlDataReader["tunitpassword"];
                        tracker.DeviceType = int.Parse((string)mySqlDataReader["ttype"]);
                        tracker.DriverName = (string)mySqlDataReader["tdrivername"];
                        tracker.Emails = (string)mySqlDataReader["temails"];
                        tracker.HttpHost = int.Parse((string)mySqlDataReader["thttphost"]);
                        tracker.Id = (int)mySqlDataReader["tid"];
                        tracker.IdlingTime = int.Parse((string)mySqlDataReader["tidlingtime"]);
                        tracker.ImageNumber = int.Parse((string)mySqlDataReader["timg"]);
                        tracker.Inputs = (string)mySqlDataReader["tinputs"];
                        tracker.MileageInitial = int.Parse((string)mySqlDataReader["tmileageInit"]);
                        tracker.MileageLimit = int.Parse((string)mySqlDataReader["tmileagelimit"]);
                        tracker.MobileDataProvider = int.Parse((string)mySqlDataReader["tprovider"]);
                        tracker.Note = (string)mySqlDataReader["tnote"];
                        tracker.OwnerName = (string)mySqlDataReader["townername"];
                        tracker.SimImei = (string)mySqlDataReader["tsimsr"];
                        tracker.SimNumber = (string)mySqlDataReader["tsimno"];
                        //tracker.Users = (string)mySqlDataReader["tusers"];
                        tracker.SpeedLimit = int.Parse((string)mySqlDataReader["tSpeedLimit"]);
                        tracker.VehicleModel = (string)mySqlDataReader["tvehiclemodel"];
                        tracker.VehicleRegistration = (string)mySqlDataReader["tvehiclereg"];


                        DataRow dataRow = dataTable.NewRow();
                        dataRow["id"] = tracker.Id;
                        dataRow["vehicleRegistration"] = tracker.VehicleRegistration;
                        dataRow["vehicleModel"] = tracker.VehicleModel;
                        dataRow["ownerName"] = tracker.OwnerName;
                        dataRow["driverName"] = tracker.DriverName;

                        dataRow["simImei"] = tracker.SimImei;
                        dataRow["simNumber"] = tracker.SimNumber;
                        dataRow["mobileDataProvider"] = tracker.MobileDataProvider;

                        dataRow["deviceImei"] = tracker.TrackerImei;
                        dataRow["devicePassword"] = tracker.DevicePassword;
                        dataRow["deviceType"] = tracker.DeviceType;

                        dataRow["emails"] = tracker.Emails;
                        dataRow["users"] = tracker.Users;

                        dataRow["mileageInitial"] = tracker.MileageInitial;
                        dataRow["mileageLimit"] = tracker.MileageLimit;
                        dataRow["SpeedLimit"] = tracker.SpeedLimit;

                        dataRow["idlingTime"] = tracker.IdlingTime;
                        dataRow["inputs"] = tracker.Inputs;
                        dataRow["imageNumber"] = tracker.ImageNumber;
                        dataRow["note"] = tracker.Note;

                        dataRow["collections"] = tracker.Collections;
                        dataRow["companyDatabaseName"] = tracker.CompanyDatabaseName;
                        dataRow["databaseHost"] = tracker.DatabaseHost;
                        dataRow["DatabaseName"] = tracker.DatabaseName;
                        dataRow["httpHost"] = tracker.HttpHost;
                        dataRow["dateCreated"] = tracker.DateTimeCreated;
                        dataRow["dateExpired"] = tracker.DateTimeExpired;

                        dataTable.Rows.Add(dataRow);
                    }


                    return dataTable;
                }
            } catch (QueryException queryException) {
                throw queryException;
            } catch (MySqlException mySqlException) {
                throw new QueryException(1, mySqlException.Message);
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {
                mysqlConnection.Close();
            }
        }

        public void fillTrackers(Company company) {
            List<Tracker> trackers = new List<Tracker>();

            try {
                mysqlConnection.Open();

                string sql =
                    "SELECT * " +
                    "FROM dbt_tracking_master.trks " +
                    "WHERE dbt_tracking_master.trks.tcmp = @CompanyDatabaseName";

                MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);
                mySqlCommand.Parameters.AddWithValue("@CompanyDatabaseName", company.DatabaseName);
                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();


                if (!mySqlDataReader.HasRows) {
                    //throw new QueryException(1, "Tracker's Collection is empty.");
                    mySqlDataReader.Dispose();
                    mysqlConnection.Close();
                } else {


                    string dateTime;

                    while (mySqlDataReader.Read()) {
                        string[] trackerUser = mySqlDataReader.GetString("tusers").Split(',');
                        List<User> trackerUsers = new List<User>();
                        foreach (User user in company.Users) {
                            for (int index = 0; index < trackerUser.ToList().Count; index++) {
                                if (Int32.Parse(trackerUser[index]) == user.Id) {
                                    trackerUsers.Add(user);
                                }
                            }
                        }

                        Tracker tracker = new Tracker();

                        dynamic dynamicCollection = JsonConvert.DeserializeObject<dynamic>(mySqlDataReader.GetString("tcollections"));
                        JArray arrayCollection = (JArray)dynamicCollection;
                        List<Collection> collections = new List<Collection>();
                        Collection collectionItem = new Collection();
                        collectionItem.Id = 0;
                        collectionItem.Name = "All";
                        collectionItem.Description = "All";
                        collections.Add(collectionItem);

                        foreach (User user in company.Users) {
                            if (user.Collections == null) {
                                continue;
                            }

                            foreach (Collection collection in user.Collections) {

                                for (int index = 0; index < arrayCollection.Count; index++) {
                                    if (dynamicCollection[index].value == collection.Id) {
                                        collections.Add(collection);
                                    }
                                }
                            }
                        }
                        tracker.Collections = collections;


                        //tracker.Collections = mySqlDataReader.GetString("tcollections");
                        tracker.CompanyDatabaseName = (string)mySqlDataReader["tcmp"];
                        tracker.DatabaseHost = int.Parse((string)mySqlDataReader["tdbhost"]);
                        tracker.DatabaseName = (string)mySqlDataReader["tdbs"];

                        dateTime = (string)mySqlDataReader["tcreatedate"];
                        tracker.DateTimeCreated = SubStandard.dateTime(dateTime);
                        dateTime = String.Empty;

                        dateTime = (string)mySqlDataReader["ttrackerexpiry"];
                        tracker.DateTimeExpired = SubStandard.dateTime(dateTime);
                        dateTime = String.Empty;

                        tracker.TrackerImei = (string)mySqlDataReader["tunit"];
                        tracker.DevicePassword = (string)mySqlDataReader["tunitpassword"];
                        tracker.DeviceType = int.Parse((string)mySqlDataReader["ttype"]);
                        tracker.DriverName = (string)mySqlDataReader["tdrivername"];
                        tracker.Emails = (string)mySqlDataReader["temails"];
                        tracker.HttpHost = int.Parse((string)mySqlDataReader["thttphost"]);
                        tracker.Id = (int)mySqlDataReader["tid"];
                        tracker.IdlingTime = int.Parse((string)mySqlDataReader["tidlingtime"]);
                        tracker.ImageNumber = int.Parse((string)mySqlDataReader["timg"]);
                        tracker.Inputs = (string)mySqlDataReader["tinputs"];
                        tracker.MileageInitial = int.Parse((string)mySqlDataReader["tmileageInit"]);
                        tracker.MileageLimit = int.Parse((string)mySqlDataReader["tmileagelimit"]);
                        tracker.MobileDataProvider = int.Parse((string)mySqlDataReader["tprovider"]);
                        tracker.Note = (string)mySqlDataReader["tnote"];
                        tracker.OwnerName = (string)mySqlDataReader["townername"];
                        tracker.SimImei = (string)mySqlDataReader["tsimsr"];
                        tracker.SimNumber = (string)mySqlDataReader["tsimno"];
                        tracker.Users = trackerUsers;
                        tracker.SpeedLimit = int.Parse((string)mySqlDataReader["tSpeedLimit"]);
                        tracker.VehicleModel = (string)mySqlDataReader["tvehiclemodel"];
                        tracker.VehicleRegistration = (string)mySqlDataReader["tvehiclereg"];

                        trackers.Add(tracker);
                    }
                    company.Trackers = trackers;
                    mySqlDataReader.Dispose();
                }
            } catch (QueryException queryException) {
                throw queryException;
            } catch (MySqlException mySqlException) {
                throw new QueryException(1, mySqlException.Message);
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {
                mysqlConnection.Close();
            }
        }
        public DataTable getTrackers(Company company) {
            DataTable dataTable = this.getReportTable(ReportType.TRACKERS);
            this.fillTrackers(company);
            List<Tracker> trackers = company.Trackers;

            foreach (Tracker tracker in trackers) {
                DataRow dataRow = dataTable.NewRow();
                dataRow["id"] = tracker.Id;
                dataRow["vehicleRegistration"] = tracker.VehicleRegistration;
                dataRow["vehicleModel"] = tracker.VehicleModel;
                dataRow["ownerName"] = tracker.OwnerName;
                dataRow["driverName"] = tracker.DriverName;

                dataRow["simImei"] = tracker.SimImei;
                dataRow["simNumber"] = tracker.SimNumber;
                dataRow["mobileDataProvider"] = tracker.MobileDataProvider;

                dataRow["deviceImei"] = tracker.TrackerImei;
                dataRow["devicePassword"] = tracker.DevicePassword;
                dataRow["deviceType"] = tracker.DeviceType;

                dataRow["emails"] = tracker.Emails;
                dataRow["users"] = tracker.Users;

                dataRow["mileageInitial"] = tracker.MileageInitial;
                dataRow["mileageLimit"] = tracker.MileageLimit;
                dataRow["SpeedLimit"] = tracker.SpeedLimit;

                dataRow["idlingTime"] = tracker.IdlingTime;
                dataRow["inputs"] = tracker.Inputs;
                dataRow["imageNumber"] = tracker.ImageNumber;
                dataRow["note"] = tracker.Note;

                dataRow["collections"] = tracker.Collections;
                dataRow["companyDatabaseName"] = tracker.CompanyDatabaseName;
                dataRow["databaseHost"] = tracker.DatabaseHost;
                dataRow["DatabaseName"] = tracker.DatabaseName;
                dataRow["httpHost"] = tracker.HttpHost;
                dataRow["dateCreated"] = tracker.DateTimeCreated;
                dataRow["dateExpired"] = tracker.DateTimeExpired;

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        //public DataTable getTrackers(Company company, User user, int userAccountId) {
        //    DataTable dataTable = this.getReportTable(ReportType.TRACKERS);

        //    try {
        //        mysqlConnection.Open();

        //        string sql =
        //            "SELECT * " +
        //            "FROM dbt_tracking_master.trks " +
        //            "WHERE dbt_tracking_master.trks.tcmp = @sCompanyName " +
        //            "AND dbt_tracking_master.trks.tusers LIKE @sUserId;";

        //        MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);

        //        mySqlCommand.Parameters.AddWithValue("@sCompanyName", company.DatabaseName.Substring(4));
        //        mySqlCommand.Parameters.AddWithValue("@sUserId", "%" + userAccountId.ToString() + "%");

        //        MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

        //        if (!mySqlDataReader.HasRows) {
        //            throw new QueryException(1, "Tracker's Collection is empty.");
        //        } else {


        //            Tracker tracker = new Tracker();
        //            string dateTime;

        //            while (mySqlDataReader.Read()) {

        //                tracker.collections = (string)mySqlDataReader["tcollections"];
        //                tracker.companyDatabaseName = (string)mySqlDataReader["tcmp"];
        //                tracker.databaseHost = int.Parse((string)mySqlDataReader["tdbhost"]);
        //                tracker.DatabaseName = (string)mySqlDataReader["tdbs"];

        //                dateTime = (string)mySqlDataReader["tcreatedate"];
        //                tracker.dateCreated = Converter.subStandardDateTimeToDateTime(dateTime);
        //                dateTime = String.Empty;

        //                dateTime = (string)mySqlDataReader["ttrackerexpiry"];
        //                tracker.dateExpired = Converter.subStandardDateTimeToDateTime(dateTime);
        //                dateTime = String.Empty;

        //                tracker.trackerImei = (string)mySqlDataReader["tunit"];
        //                tracker.devicePassword = (string)mySqlDataReader["tunitpassword"];
        //                tracker.deviceType = int.Parse((string)mySqlDataReader["ttype"]);
        //                tracker.driverName = (string)mySqlDataReader["tdrivername"];
        //                tracker.emails = (string)mySqlDataReader["temails"];
        //                tracker.httpHost = int.Parse((string)mySqlDataReader["thttphost"]);
        //                tracker.id = (int)mySqlDataReader["tid"];
        //                tracker.idlingTime = int.Parse((string)mySqlDataReader["tidlingtime"]);
        //                tracker.imageNumber = int.Parse((string)mySqlDataReader["timg"]);
        //                tracker.inputs = (string)mySqlDataReader["tinputs"];
        //                tracker.mileageInitial = int.Parse((string)mySqlDataReader["tmileageInit"]);
        //                tracker.mileageLimit = int.Parse((string)mySqlDataReader["tmileagelimit"]);
        //                tracker.mobileDataProvider = int.Parse((string)mySqlDataReader["tprovider"]);
        //                tracker.note = (string)mySqlDataReader["tnote"];
        //                tracker.ownerName = (string)mySqlDataReader["townername"];
        //                tracker.simImei = (string)mySqlDataReader["tsimsr"];
        //                tracker.simNumber = (string)mySqlDataReader["tsimno"];
        //                tracker.users = (string)mySqlDataReader["tusers"];
        //                tracker.SpeedLimit = int.Parse((string)mySqlDataReader["tSpeedLimit"]);
        //                tracker.vehicleModel = (string)mySqlDataReader["tvehiclemodel"];
        //                tracker.vehicleRegistration = (string)mySqlDataReader["tvehiclereg"];


        //                DataRow dataRow = dataTable.NewRow();
        //                dataRow["id"] = tracker.id;
        //                dataRow["vehicleRegistration"] = tracker.vehicleRegistration;
        //                dataRow["vehicleModel"] = tracker.vehicleModel;
        //                dataRow["ownerName"] = tracker.ownerName;
        //                dataRow["driverName"] = tracker.driverName;

        //                dataRow["simImei"] = tracker.simImei;
        //                dataRow["simNumber"] = tracker.simNumber;
        //                dataRow["mobileDataProvider"] = tracker.mobileDataProvider;

        //                dataRow["deviceImei"] = tracker.trackerImei;
        //                dataRow["devicePassword"] = tracker.devicePassword;
        //                dataRow["deviceType"] = tracker.deviceType;

        //                dataRow["emails"] = tracker.emails;
        //                dataRow["users"] = tracker.users;

        //                dataRow["mileageInitial"] = tracker.mileageInitial;
        //                dataRow["mileageLimit"] = tracker.mileageLimit;
        //                dataRow["SpeedLimit"] = tracker.SpeedLimit;

        //                dataRow["idlingTime"] = tracker.idlingTime;
        //                dataRow["inputs"] = tracker.inputs;
        //                dataRow["imageNumber"] = tracker.imageNumber;
        //                dataRow["note"] = tracker.note;

        //                dataRow["collections"] = tracker.collections;
        //                dataRow["companyDatabaseName"] = tracker.companyDatabaseName;
        //                dataRow["databaseHost"] = tracker.databaseHost;
        //                dataRow["DatabaseName"] = tracker.DatabaseName;
        //                dataRow["httpHost"] = tracker.httpHost;
        //                dataRow["dateCreated"] = tracker.dateCreated;
        //                dataRow["dateExpired"] = tracker.dateExpired;

        //                dataTable.Rows.Add(dataRow);
        //            }


        //            return dataTable;
        //        }
        //    } catch (QueryException queryException) {
        //        throw queryException;
        //    } catch (MySqlException mySqlException) {
        //        throw new QueryException(1, mySqlException.Message);
        //    } catch (Exception exception) {
        //        throw new QueryException(1, exception.Message);
        //    } finally {
        //        mysqlConnection.Close();
        //    }
        //}


        public void getCompany(Company company) {
            try {
                mysqlConnection.Open();

                string sql =
                    "SELECT * " +
                    "FROM dbt_tracking_master.cmps " +
                    "WHERE dbt_tracking_master.cmps.cmpname = @sCompanyName;";

                MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);
                mySqlCommand.Parameters.AddWithValue("@sCompanyName", company.Username);

                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();



                if (!mySqlDataReader.HasRows) {
                    mySqlDataReader.Dispose();
                    throw new QueryException(1, "Company does not exist.");
                } else {
                    mySqlDataReader.Read();
                    company.Id = mySqlDataReader.GetInt32("cmpid");
                    company.Username = mySqlDataReader.GetString("cmpname");
                    company.DisplayName = mySqlDataReader.GetString("cmpdisplayname");
                    company.Host = int.Parse(mySqlDataReader.GetString("cmphost"));
                    company.Email = mySqlDataReader.GetString("cmpemail");
                    company.PhoneNo = mySqlDataReader.GetString("cmpphoneno");
                    company.MobileNo = mySqlDataReader.GetString("cmpmobileno");
                    company.Address = mySqlDataReader.GetString("cmpaddress");
                    company.IsActive = (mySqlDataReader.GetString("cmpactive") == "1") ? true : false;
                    company.DateTimeCreated = SubStandard.dateTime(mySqlDataReader.GetString("cmpcreatedate"));
                    company.DateTimeExpired = SubStandard.dateTime(mySqlDataReader.GetString("cmpexpiredate"));
                    company.DatabaseName = mySqlDataReader.GetString("cmpdbname");

                    mySqlDataReader.Dispose();
                }
            } catch (MySqlException mySqlException) {
                throw new QueryException(1, mySqlException.Message);
            } catch (QueryException queryException) {
                throw queryException;
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {
                mysqlConnection.Close();
            }

        }
        public void getUser(Company company, User user) {
            try {
                mysqlConnection = new MySqlConnection(database.getConnectionString());

                mysqlConnection.Open();

                string sql =
                    "SELECT * " +
                    "FROM cmp_" + company.DatabaseName + ".usrs " +
                    "WHERE " +
                    "cmp_" + company.DatabaseName + ".usrs.uname = @sUsername AND " +
                    "cmp_" + company.DatabaseName + ".usrs.upass = @sPassword;";

                MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);
                mySqlCommand.Parameters.AddWithValue("@sUsername", user.Username);
                String coded = Cryptography.md5(user.Password);
                mySqlCommand.Parameters.AddWithValue("@sPassword", coded);

                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                if (!mySqlDataReader.HasRows) {
                    mySqlDataReader.Dispose();
                    throw new QueryException(1, "Username or Password does not exist.");
                } else {
                    mySqlDataReader.Read();

                    user.Id = mySqlDataReader.GetInt32("uid");
                    user.Username = mySqlDataReader.GetString("uname");
                    //user.Password = mySqlDataReader.GetString("upass");
                    user.Email = mySqlDataReader.GetString("uemail");
                    user.Main = mySqlDataReader.GetString("umain");
                    user.AccessLevel = int.Parse(mySqlDataReader.GetString("upriv"));
                    user.Timezone = mySqlDataReader.GetString("utimezone");
                    user.IsActive = mySqlDataReader.GetString("uactive").Equals("1");
                    user.DatabaseName = mySqlDataReader.GetString("udbs");

                    if (!String.IsNullOrEmpty(mySqlDataReader.GetString("uexpiredate"))) {
                        string dateTime = (mySqlDataReader.GetString("uexpiredate"));
                        if (!String.IsNullOrEmpty(dateTime)) {
                            DateTime parsedDate = SubStandard.dateTime(dateTime);
                            user.DateTimeExpired = parsedDate;
                        }
                    } else {
                        user.DateTimeExpired = new DateTime(2050, 01, 01);
                    }

                    if (!String.IsNullOrEmpty(mySqlDataReader.GetString("ucreatedate"))) {
                        string dateTime = mySqlDataReader.GetString("ucreatedate");
                        if (!String.IsNullOrEmpty(dateTime)) {
                            DateTime parsedDate = SubStandard.dateTime(dateTime);
                            user.DateTimeCreated = parsedDate;
                        }
                    } else {
                        user.DateTimeCreated = new DateTime(2010, 01, 01);
                    }
                    mySqlDataReader.Dispose();
                }

            } catch (MySqlException mySqlException) {
                throw new QueryException(1, mySqlException.Message);
            } catch (QueryException queryException) {
                throw queryException;
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {
                mysqlConnection.Close();
            }
        }
        public void fillUsers(Company company, User user) {
            List<User> users = new List<User>();
            try {
                mysqlConnection = new MySqlConnection(database.getConnectionString());

                mysqlConnection.Open();

                string sql = "";
                if (user.AccessLevel == 1 || user.AccessLevel == 2) {
                    sql =
                        "SELECT * " +
                        "FROM cmp_" + company.DatabaseName + ".usrs " +
                        "WHERE cmp_" + company.DatabaseName + ".usrs.upriv >= " + user.AccessLevel.ToString() + ";";
                } else {
                    sql =
                          "SELECT * " +
                          "FROM cmp_" + company.DatabaseName + ".usrs " +
                          "WHERE cmp_" + company.DatabaseName + ".usrs.upriv = " + user.AccessLevel.ToString() +
                          " and cmp_" + company.DatabaseName + ".usrs.uname = " + user.Username + ";";
                }

                MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);

                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                if (!mySqlDataReader.HasRows) {
                    mySqlDataReader.Dispose();
                    throw new QueryException(1, "Users table is empty.");
                } else {
                    while (mySqlDataReader.Read()) {
                        User userSubordinate = new User();
                        userSubordinate.Id = mySqlDataReader.GetInt32("uid");
                        userSubordinate.Username = mySqlDataReader.GetString("uname");
                        userSubordinate.Password = mySqlDataReader.GetString("upass");
                        userSubordinate.Email = mySqlDataReader.GetString("uemail");
                        userSubordinate.Main = mySqlDataReader.GetString("umain");
                        userSubordinate.AccessLevel = int.Parse(mySqlDataReader.GetString("upriv"));
                        userSubordinate.Timezone = mySqlDataReader.GetString("utimezone");
                        userSubordinate.IsActive = mySqlDataReader.GetString("uactive").Equals("1");
                        userSubordinate.DatabaseName = mySqlDataReader.GetString("udbs");

                        if (!String.IsNullOrEmpty(mySqlDataReader.GetString("uexpiredate"))) {
                            string dateTime = (mySqlDataReader.GetString("uexpiredate"));
                            if (!String.IsNullOrEmpty(dateTime)) {
                                DateTime parsedDate = SubStandard.dateTime(dateTime);
                                userSubordinate.DateTimeExpired = parsedDate;
                            }
                        } else {
                            userSubordinate.DateTimeExpired = new DateTime(2050, 01, 01);
                        }

                        if (!String.IsNullOrEmpty(mySqlDataReader.GetString("ucreatedate"))) {
                            string dateTime = mySqlDataReader.GetString("ucreatedate");
                            if (!String.IsNullOrEmpty(dateTime)) {
                                DateTime parsedDate = SubStandard.dateTime(dateTime);
                                userSubordinate.DateTimeCreated = parsedDate;
                            }
                        } else {
                            userSubordinate.DateTimeCreated = new DateTime(2010, 01, 01);
                        }
                        users.Add(userSubordinate);
                    }
                    mySqlDataReader.Dispose();
                }

            } catch (MySqlException mySqlException) {
                throw new QueryException(1, mySqlException.Message);
            } catch (QueryException queryException) {
                throw queryException;
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {
                mysqlConnection.Close();
            }
            company.Users = users;
        }

        public void fillCollection(Company company) {
            foreach (User user in company.Users) {
                List<Collection> collections = new List<Collection>();
                try {
                    mysqlConnection = new MySqlConnection(database.getConnectionString());

                    mysqlConnection.Open();

                    string sql =
                        "SELECT * " +
                        "FROM cmp_" + company.DatabaseName + ".coll_" + user.DatabaseName + ";";

                    MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);

                    MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                    if (!mySqlDataReader.HasRows) {
                        Collection collection = new Collection();
                        collection.Id = 0;
                        collection.Name = "All";
                        collection.Description = "All";
                        collections.Add(collection);
                        user.Collections = collections;
                        mySqlDataReader.Dispose();
                    } else {
                        Collection collection = new Collection();
                        collection.Id = 0;
                        collection.Name = "All";
                        collection.Description = "All";
                        collections.Add(collection);

                        while (mySqlDataReader.Read()) {
                            Collection collectionItem = new Collection();
                            collectionItem.Id = mySqlDataReader.GetInt32("collid");
                            collectionItem.Name = mySqlDataReader.GetString("collname");
                            collectionItem.Description = mySqlDataReader.GetString("colldesc");
                            collections.Add(collectionItem);
                        }

                        user.Collections = collections;
                        mySqlDataReader.Dispose();
                    }
                } catch (MySqlException mySqlException) {
                    throw new QueryException(1, mySqlException.Message);
                } catch (QueryException queryException) {
                    throw queryException;
                } catch (Exception exception) {
                    throw new QueryException(1, exception.Message);
                } finally {
                    mysqlConnection.Close();
                }
            }
        }
        public void fillPois(Company company) {
            foreach (User user in company.Users) {
                List<Poi> pois = new List<Poi>();
                try {
                    mysqlConnection = new MySqlConnection(database.getConnectionString());

                    mysqlConnection.Open();

                    string sql =
                        "SELECT * " +
                        "FROM cmp_" + company.DatabaseName + ".poi_" + user.DatabaseName + ";";

                    MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);

                    MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                    if (!mySqlDataReader.HasRows) {
                        mySqlDataReader.Dispose();
                    } else {
                        while (mySqlDataReader.Read()) {
                            //Poi poi = new Poi();
                            //poi.id = mySqlDataReader.GetInt32("poi_id");
                            //poi.name = mySqlDataReader.GetString("poi_name");
                            //poi.description = mySqlDataReader.GetString("poi_desc");
                            //poi.image = mySqlDataReader.GetString("poi_img");
                            //poi.location = new Coordinate(double.Parse(mySqlDataReader.GetString("poi_lat")),
                            //                            double.Parse(mySqlDataReader.GetString("poi_lon")));
                            //pois.Add(poi);
                            Poi poi = new Poi();
                            poi.Id = mySqlDataReader.GetInt32("poi_id");
                            poi.Name = mySqlDataReader.GetString("poi_name");
                            poi.Description = mySqlDataReader.GetString("poi_desc");
                            poi.Image = mySqlDataReader.GetString("poi_img");

                            string poiLatitude = mySqlDataReader.GetString("poi_lat");
                            string potLongitude = mySqlDataReader.GetString("poi_lon");

                            if (!String.IsNullOrEmpty(poiLatitude) || !String.IsNullOrEmpty(potLongitude)) {
                                poi.Coordinate = new Coordinate(
                                        double.Parse(poiLatitude),
                                        double.Parse(potLongitude)
                                        );
                            }


                            pois.Add(poi);
                        }

                        user.Pois = pois;
                        mySqlDataReader.Dispose();
                    }
                } catch (MySqlException mySqlException) {
                    throw new QueryException(1, mySqlException.Message);
                } catch (QueryException queryException) {
                    throw queryException;
                } catch (Exception exception) {
                    throw new QueryException(1, exception.Message);
                } finally {
                    mysqlConnection.Close();
                }
            }
        }
        public void fillGeofences(Company company) {
            List<Geofence> geofences = new List<Geofence>();

            try {
                mysqlConnection = new MySqlConnection(database.getConnectionString());
                mysqlConnection.Open();

                string sql =
                     "SELECT * " +
                     "FROM cmp_" + company.DatabaseName + ".gf";

                MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);
                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                if (!mySqlDataReader.HasRows) {
                    mySqlDataReader.Dispose();
                } else {
                    while (mySqlDataReader.Read()) {
                        Geofence geofence = new Geofence();
                        geofence.Id = mySqlDataReader.GetInt32("gf_id");
                        geofence.Name = mySqlDataReader.GetString("gf_name");
                        geofence.Tracks = mySqlDataReader.GetString("gf_trks");

                        string geofenceData = (string)mySqlDataReader["gf_data"];
                        geofenceData = geofenceData.Replace("),( ", "|");
                        geofenceData = geofenceData.Replace(")", string.Empty);
                        geofenceData = geofenceData.Replace("(", string.Empty);
                        geofenceData = geofenceData.Replace(" ", string.Empty);


                        List<string> points = geofenceData.Split('|').ToList();
                        foreach (string point in points) {
                            string[] coordinates = point.Split(',');
                            double latitude = double.Parse(coordinates[0]);
                            double longitude = double.Parse(coordinates[1]);
                            Coordinate location = new Coordinate(latitude, longitude);
                            geofence.addPoint(location);
                        }
                        geofences.Add(geofence);
                    }
                    company.Geofences = geofences;
                    mySqlDataReader.Dispose();
                }
            } catch (MySqlException mySqlException) {
                throw new QueryException(1, mySqlException.Message);
            } catch (QueryException queryException) {
                throw queryException;
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {
                mysqlConnection.Close();
            }
        }
        //Based Reports
        public int getTrackerHistoricalDataCount(User account, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, Tracker tracker) {
            DataTable dataTable = new DataTable();
            try {
                mysqlConnection.Open();

                double timestampFrom = Ats.Helper.Converter.dateTimeToUnixTimestamp(dateTimeDateFrom);
                double timestampTo = Ats.Helper.Converter.dateTimeToUnixTimestamp(dateTimeDateTo);

                string sql = "";
                if (timestampFrom > timestampTo) {
                    sql =
                       "SELECT  COUNT(*) " +
                       "FROM trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + " " +
                       "WHERE trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_time >= " + timestampTo.ToString() + " " +
                       "AND trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_time <= " + timestampFrom.ToString() + " " +
                       "ORDER BY trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_id ASC;";


                } else {
                    sql =
                          "SELECT  COUNT(*) " +
                          "FROM trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + " " +
                          "WHERE trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_time <= " + timestampTo.ToString() + " " +
                          "AND trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_time >= " + timestampFrom.ToString() + " " +
                          "ORDER BY trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_id ASC;";

                }
                MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);

                int count = Convert.ToInt32(mySqlCommand.ExecuteScalar());
                return count;

            } catch (QueryException queryException) {
                throw queryException;
            } catch (MySqlException mySqlException) {
                throw mySqlException;
            } catch (Exception exception) {
                throw exception;
            } finally {
                mysqlConnection.Close();
            }


        }
        public DataTable getTrackerHistoricalData(Company company, User user, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, int limit, int offset, Tracker tracker) {
            DataTable dataTable = this.getReportTable(ReportType.HISTORICAL);
            dataTable.TableName = tracker.DatabaseName;

            try {
                mysqlConnection.Open();

                double timestampFrom = Ats.Helper.Converter.dateTimeToUnixTimestamp(dateTimeDateFrom);
                double timestampTo = Ats.Helper.Converter.dateTimeToUnixTimestamp(dateTimeDateTo);

                string sql = "";
                if (timestampFrom > timestampTo) {
                    sql =
                       "SELECT " +

                       "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_id, " +
                       "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_time, " +
                       "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_lat, " +
                       "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_lng, " +
                       "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_ori, " +
                       "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_speed, " +
                       "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_mileage, " +
                       "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_data " +


                       "FROM trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + " " +
                       "WHERE trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_time >= " + timestampTo.ToString() + " " +
                       "AND trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_time <= " + timestampFrom.ToString() + " " +
                       "ORDER BY trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_time ASC LIMIT " + limit.ToString() + " OFFSET " + offset.ToString() + ";";
                } else {
                    sql =
                        "SELECT " +

                        "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_id, " +
                        "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_time, " +
                        "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_lat, " +
                        "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_lng, " +
                        "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_ori, " +
                        "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_speed, " +
                        "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_mileage, " +
                        "trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_data " +

                        "FROM trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + " " +
                        "WHERE trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_time <= " + timestampTo.ToString() + " " +
                        "AND trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_time >= " + timestampFrom.ToString() + " " +
                        "ORDER BY trk_" + tracker.DatabaseName + ".gps_" + tracker.DatabaseName + ".gm_time ASC LIMIT " + limit.ToString() + " OFFSET " + offset.ToString() + ";";
                }
                MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);

                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();



                if (!mySqlDataReader.HasRows) {
                    //throw new QueryException(1, "Tracker data is empty.");
                    return dataTable;
                } else {
                    int index = offset;

                    DataRow dataRow;
                    DataRow dataRowBefore = dataTable.NewRow();


                    while (mySqlDataReader.Read()) {
                        index++;

                        dataRow = dataTable.NewRow();
                        dataRow["No"] = index;
                        dataRow["DateTime"] = Converter.unixTimeStampToDateTime(double.Parse((string)mySqlDataReader["gm_time"]));

                        double latitude = double.Parse((string)mySqlDataReader["gm_lat"]);
                        double longitude = double.Parse((string)mySqlDataReader["gm_lng"]);

                        dataRow["Latitude"] = latitude;
                        dataRow["Longitude"] = longitude;
                        dataRow["Speed"] = int.Parse((string)mySqlDataReader["gm_speed"]);
                        dataRow["Mileage"] = double.Parse((string)mySqlDataReader["gm_mileage"]);

                        if ((double)dataRow["Mileage"] < 0) {
                            dataRow["Mileage"] = ((double)dataRow["Mileage"]) * -1;
                        }

                        if (dataRowBefore["Mileage"] != System.DBNull.Value) {
                            double mileAgeObtained = (double)dataRow["Mileage"] - (double)dataRowBefore["Mileage"];
                            dataRow["Distance"] = mileAgeObtained;
                            dataRow["Fuel"] = mileAgeObtained / Settings.Default.fuelLiterToKilometer;
                            dataRow["Cost"] = ((double)dataRow["Fuel"]) * Settings.Default.fuelLiterToCost;
                        } else {
                            dataRow["Distance"] = 0.00f;
                            dataRow["Fuel"] = 0.00f;
                            dataRow["Cost"] = 0.00f;
                        }
                        dataRow["Degrees"] = int.Parse((string)mySqlDataReader["gm_ori"]);
                        dataRow["Direction"] = Converter.degreesToCardinalDetailed(double.Parse((string)mySqlDataReader["gm_ori"]));

                        //1,			            //                                                          (0)
                        //35,			            //Event code(Decimal)
                        //11,			            //Number of satellites(Decimal)
                        //26,			            //GSM signal status(Decimal)
                        //17160691, 		        //Mileage(Decimal)unit: meter
                        //0.7, 			            //hpos accuracy(Decimal)
                        //18, 			            //Altitude(Decimal)unit: meter
                        //18661240, 		        //Run time(Decimal)unit: second
                        //427|2|0078|283F, 	        //Base station information(binary|binary|hex|hex)           (8)
                        //==============================================0200
                        //0,0,0,0,0,0,0,0,          //Io port lowbyte (low bit start from left)                 (9)
                        //0,1,0,0,0,0,0,0,          //Io port lowbyte (low bit start from left)                 (17)
                        //==============================================
                        //000B,0000,0000,0A6E,0434, //Analog input value                                        (25)
                        //00000001 		            //System mark

                        string gmData = (string)mySqlDataReader["gm_data"];
                        string[] data = gmData.Split(',');
                        dataRow["EventCode"] = Enum.GetName(typeof(EventCode), (EventCode)int.Parse(data[1]));
                        dataRow["GpsSatellites"] = int.Parse(data[2]);
                        dataRow["GsmSignal"] = int.Parse(data[3]);
                        dataRow["Altitude"] = int.Parse(data[6]);

                        dataRow["ACC"] = (int.Parse(data[18]) == 1) ? true : false;
                        dataRow["SOS"] = (int.Parse(data[17]) == 1) ? true : false;
                        dataRow["OverSpeed"] = ((int)dataRow["Speed"] > tracker.SpeedLimit) ? true : false;
                        //Geofence
                        Coordinate Coordinate = new Coordinate(latitude, longitude);
                        //MapTools mapTools = new MapTools();

                        if (company.Geofences != null) {
                            foreach (Geofence geofence in company.Geofences) {
                                if (Geofence.isPointInPolygon(geofence, Coordinate)) {
                                    if (String.IsNullOrEmpty(geofence.Name)) {
                                        dataRow["Geofence"] = "";
                                    } else {
                                        dataRow["Geofence"] = geofence.Name;
                                    }
                                }
                            }
                        }

                        double batteryStrength = (double)int.Parse(data[28], System.Globalization.NumberStyles.AllowHexSpecifier);
                        batteryStrength = ((batteryStrength - 2114f) * (100f / 492f));//*100.0;

                        if (batteryStrength > 100) {
                            batteryStrength = 100f;
                        } else if (batteryStrength < 0) {
                            batteryStrength = 0;
                        }

                        double batteryVoltage = (double)int.Parse(data[28], System.Globalization.NumberStyles.AllowHexSpecifier);
                        batteryVoltage = (batteryVoltage * 3 * 2) / 1024;

                        double externalVoltage = (double)int.Parse(data[29], System.Globalization.NumberStyles.AllowHexSpecifier);
                        externalVoltage = (externalVoltage * 3 * 16) / 1024;


                        dataRow["Battery"] = batteryStrength;
                        dataRow["BatteryVoltage"] = batteryVoltage;
                        dataRow["ExternalVoltage"] = externalVoltage;

                        dataTable.Rows.Add(dataRow);
                        dataRowBefore = dataRow;
                    }
                    mySqlDataReader.Close();
                    return dataTable;
                }
            } catch (QueryException queryException) {
                throw queryException;
            } catch (MySqlException mySqlException) {
                throw mySqlException;
            } catch (Exception exception) {
                throw exception;
            } finally {
                mysqlConnection.Close();
            }


        }
        //Processed Reports
        public DataTable getTrackerRunningData(Company company, User user, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, int limit, int offset, Tracker tracker) {
            DataTable dataTableHistoricalData = this.getTrackerHistoricalData(company, user, dateTimeDateFrom, dateTimeDateTo, limit, offset, tracker);
            DataTable dataTableRunningData = this.getReportTable(ReportType.RUNNING);
            dataTableRunningData.TableName = tracker.DatabaseName;

            try {

                bool? runningStatusNow = null;
                bool? runningStatusBefore = null;
                EventCode eventCode = EventCode.TRACK_BY_TIME_INTERVAL;
                bool acc = false;

                int index = 0;

                double speed = 0;
                double speedDivisor = 0;
                double speedAccumulator = 0;
                double speedMax = 0;
                double speedAverage = 0;

                double distanceCovered = 0;
                double distance = 0;


                String geofenceFrom = "";
                String geofenceTo = "";

                DateTime dateTimeRunningFrom = new DateTime();
                DateTime dateTimeRunningTo = new DateTime();

                double latitudeTo = 0;
                double latitudeFrom = 0;
                double longitudeTo = 0;
                double longitudeFrom = 0;

                TimeSpan timeSpan;

                for (int no = 0; no < dataTableHistoricalData.Rows.Count; no++) {
                    //Initialized
                    eventCode = (dataTableHistoricalData.Rows[no]["EventCode"] == System.DBNull.Value) ?
                    EventCode.TRACK_BY_TIME_INTERVAL : (EventCode)Enum.Parse(typeof(EventCode), (string)dataTableHistoricalData.Rows[no]["EventCode"], true);
                    acc = (bool)dataTableHistoricalData.Rows[no]["ACC"];
                    speed = (int)dataTableHistoricalData.Rows[no]["Speed"];
                    distance = (double)dataTableHistoricalData.Rows[no]["Distance"];

                    //Conditions
                    if (acc == true && speed > 0) {
                        runningStatusNow = true;
                    } else {
                        runningStatusNow = false;
                    }

                    //if (acc = false || speed <= 0) {
                    //    runningStatusNow = false;
                    //}

                    //One time
                    if ((int)dataTableHistoricalData.Rows[no]["No"] == 1) {
                        dateTimeRunningFrom = dateTimeDateFrom;//(DateTime)dataTableHistoricalData.Rows[no]["DateTime"];

                        latitudeFrom = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeFrom = (double)dataTableHistoricalData.Rows[no]["Longitude"];
                        geofenceFrom = (dataTableHistoricalData.Rows[no]["Geofence"] == System.DBNull.Value) ? "OUTSIDE" : (string)dataTableHistoricalData.Rows[no]["Geofence"];
                        runningStatusBefore = runningStatusNow;
                    }

                    if (runningStatusNow != runningStatusBefore) {
                        index++;
                        dateTimeRunningTo = (DateTime)dataTableHistoricalData.Rows[no]["DateTime"];
                        latitudeTo = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeTo = (double)dataTableHistoricalData.Rows[no]["Longitude"];
                        geofenceTo = (dataTableHistoricalData.Rows[no]["Geofence"] == System.DBNull.Value) ? "OUTSIDE" : (string)dataTableHistoricalData.Rows[no]["Geofence"];

                        timeSpan = dateTimeRunningTo - dateTimeRunningFrom;

                        speedAverage = speedAccumulator / speedDivisor;
                        speedAccumulator = 0;
                        speedDivisor = 0;

                        DataRow dataRowRunningData = dataTableRunningData.NewRow();
                        dataRowRunningData["No"] = index;
                        dataRowRunningData["Status"] = runningStatusBefore;

                        dataRowRunningData["LatitudeFrom"] = latitudeFrom;
                        dataRowRunningData["LatitudeTo"] = latitudeTo;
                        dataRowRunningData["LongitudeFrom"] = longitudeFrom;
                        dataRowRunningData["LongitudeTo"] = longitudeTo;

                        dataRowRunningData["DateTimeFrom"] = dateTimeRunningFrom;
                        dataRowRunningData["DateTimeTo"] = dateTimeRunningTo;
                        dataRowRunningData["Time"] = timeSpan;
                        dataRowRunningData["Distance"] = distanceCovered;

                        dataRowRunningData["Fuel"] = distanceCovered / Settings.Default.fuelLiterToKilometer;
                        dataRowRunningData["Cost"] = (double)dataRowRunningData["Fuel"] / Settings.Default.fuelLiterToCost;

                        if (geofenceTo.Equals(geofenceFrom)) {
                            dataRowRunningData["Geofences"] = geofenceFrom;
                        } else {
                            dataRowRunningData["Geofences"] = geofenceFrom + " to " + geofenceTo;
                        }
                        dataRowRunningData["SpeedMax"] = speedMax;
                        dataRowRunningData["SpeedAve"] = speedAverage;

                        speedMax = 0;
                        distanceCovered = 0;
                        dateTimeRunningFrom = dateTimeRunningTo;
                        longitudeFrom = longitudeTo;
                        latitudeFrom = latitudeTo;
                        geofenceFrom = geofenceTo;
                        dataTableRunningData.Rows.Add(dataRowRunningData);
                        runningStatusBefore = runningStatusNow;
                    }
                    if (no == dataTableHistoricalData.Rows.Count - 1) {
                        index++;
                        dateTimeRunningTo = dateTimeDateTo;//(DateTime)dataTableHistoricalData.Rows[no]["DateTime"];
                        latitudeTo = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeTo = (double)dataTableHistoricalData.Rows[no]["Longitude"];
                        geofenceTo = (dataTableHistoricalData.Rows[no]["Geofence"] == System.DBNull.Value) ? "OUTSIDE" : (string)dataTableHistoricalData.Rows[no]["Geofence"];

                        timeSpan = dateTimeRunningTo - dateTimeRunningFrom;

                        speedAverage = speedAccumulator / 1;
                        speedAccumulator = 0;
                        speedDivisor = 0;

                        DataRow dataRowRunningData = dataTableRunningData.NewRow();
                        dataRowRunningData["No"] = index;
                        dataRowRunningData["Status"] = runningStatusNow;

                        dataRowRunningData["DateTimeFrom"] = dateTimeRunningFrom;
                        dataRowRunningData["DateTimeTo"] = dateTimeRunningTo;

                        dataRowRunningData["LatitudeFrom"] = latitudeFrom;
                        dataRowRunningData["LatitudeTo"] = latitudeTo;
                        dataRowRunningData["LongitudeFrom"] = longitudeFrom;
                        dataRowRunningData["Longitudeto"] = longitudeTo;

                        dataRowRunningData["Time"] = timeSpan;
                        dataRowRunningData["Distance"] = distanceCovered;

                        dataRowRunningData["Fuel"] = distanceCovered / Settings.Default.fuelLiterToKilometer;
                        dataRowRunningData["Cost"] = (double)dataRowRunningData["Fuel"] / Settings.Default.fuelLiterToCost;

                        if (geofenceTo.Equals(geofenceFrom)) {
                            dataRowRunningData["Geofences"] = geofenceFrom;
                        } else {
                            dataRowRunningData["Geofences"] = geofenceFrom + " to " + geofenceTo;
                        }
                        dataRowRunningData["SpeedMax"] = speedMax;
                        dataRowRunningData["SpeedAve"] = speedAverage;

                        speedMax = 0;
                        distanceCovered = 0;
                        dateTimeRunningFrom = dateTimeRunningTo;
                        longitudeFrom = longitudeTo;
                        latitudeFrom = latitudeTo;
                        geofenceFrom = geofenceTo;
                        dataTableRunningData.Rows.Add(dataRowRunningData);
                        runningStatusBefore = runningStatusNow;
                    }

                    //Accumulators : SpeedAve, SpeedMax, Distance 
                    speedDivisor++;
                    speedAccumulator += speed;
                    if (speed > speedMax) {
                        speedMax = speed;
                    }
                    distanceCovered += distance;

                }
            } catch (QueryException queryException) {
                throw queryException;
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {

            }
            return dataTableRunningData;
        }

        public DataTable getTrackerIdlingData(Company company, User user, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, int limit, int offset, Tracker tracker) {
            DataTable dataTableHistoricalData = this.getTrackerHistoricalData(company, user, dateTimeDateFrom, dateTimeDateTo, limit, offset, tracker);
            DataTable dataTableIdleData = this.getReportTable(ReportType.IDLING);
            dataTableIdleData.TableName = tracker.DatabaseName;

            try {

                bool? idleStatusNow = null;
                bool? idleStatusBefore = null;

                EventCode eventCode = EventCode.TRACK_BY_TIME_INTERVAL;
                bool? acc = null;

                int index = 0;

                double speed = 0;
                double speedDivisor = 0;
                double speedAccumulator = 0;
                double speedMax = 0;
                double speedAverage = 0;

                double distanceCovered = 0;
                double distance = 0;

                String geofenceFrom = "";
                String geofenceTo = "";

                DateTime dateTimeIdleFrom = new DateTime();
                DateTime dateTimeIdleTo = new DateTime();

                double latitudeTo = 0;
                double latitudeFrom = 0;
                double longitudeTo = 0;
                double longitudeFrom = 0;

                TimeSpan timeSpan;

                for (int no = 0; no < dataTableHistoricalData.Rows.Count; no++) {

                    //Initialized
                    eventCode = (dataTableHistoricalData.Rows[no]["EventCode"] == System.DBNull.Value) ?
                       EventCode.TRACK_BY_TIME_INTERVAL : (EventCode)Enum.Parse(typeof(EventCode), (string)dataTableHistoricalData.Rows[no]["EventCode"], true);
                    acc = (bool)dataTableHistoricalData.Rows[no]["ACC"];
                    speed = (int)dataTableHistoricalData.Rows[no]["Speed"];
                    distance = (double)dataTableHistoricalData.Rows[no]["Distance"];
                    geofenceTo = (dataTableHistoricalData.Rows[no]["Geofence"] == System.DBNull.Value) ? "OUTSIDE" : (string)dataTableHistoricalData.Rows[no]["Geofence"];

                    //Conditions
                    if (speed == 0 && acc == true) {
                        idleStatusNow = true;
                    } else {
                        idleStatusNow = false;
                    }

                    //One Time
                    if ((int)dataTableHistoricalData.Rows[no]["No"] == 1) {
                        dateTimeIdleFrom = dateTimeDateFrom;//(DateTime)dataTableHistoricalData.Rows[no]["DateTime"];
                        latitudeFrom = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeFrom = (double)dataTableHistoricalData.Rows[no]["Longitude"];
                        geofenceFrom = (dataTableHistoricalData.Rows[no]["Geofence"] == System.DBNull.Value) ? "OUTSIDE" : (string)dataTableHistoricalData.Rows[no]["Geofence"];
                        idleStatusBefore = idleStatusNow;
                    }

                    if (idleStatusBefore != idleStatusNow) {
                        index++;

                        dateTimeIdleTo = (DateTime)dataTableHistoricalData.Rows[no]["DateTime"];
                        latitudeTo = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeTo = (double)dataTableHistoricalData.Rows[no]["Longitude"];
                        geofenceTo = (dataTableHistoricalData.Rows[no]["Geofence"] == System.DBNull.Value) ? "OUTSIDE" : (string)dataTableHistoricalData.Rows[no]["Geofence"];

                        timeSpan = dateTimeIdleTo - dateTimeIdleFrom;


                        speedAverage = speedAccumulator / speedDivisor;
                        speedAccumulator = 0;
                        speedDivisor = 0;

                        DataRow dataRowIdleData = dataTableIdleData.NewRow();
                        dataRowIdleData["No"] = index;
                        dataRowIdleData["Status"] = idleStatusBefore;
                        dataRowIdleData["DateTimeFrom"] = dateTimeIdleFrom;
                        dataRowIdleData["DateTimeTo"] = dateTimeIdleTo;

                        dataRowIdleData["LatitudeFrom"] = latitudeFrom;
                        dataRowIdleData["LatitudeTo"] = latitudeTo;
                        dataRowIdleData["LongitudeFrom"] = longitudeFrom;
                        dataRowIdleData["LongitudeTo"] = longitudeTo;


                        dataRowIdleData["Time"] = timeSpan;
                        dataRowIdleData["Distance"] = distanceCovered;

                        if (geofenceTo.Equals(geofenceFrom)) {
                            dataRowIdleData["Geofences"] = geofenceFrom;
                        } else {
                            dataRowIdleData["Geofences"] = geofenceFrom + " to " + geofenceTo;
                        }

                        dataRowIdleData["Fuel"] = distanceCovered / Settings.Default.fuelLiterToKilometer;
                        dataRowIdleData["Cost"] = (double)dataRowIdleData["Fuel"] / Settings.Default.fuelLiterToCost;

                        dataRowIdleData["SpeedMax"] = speedMax;
                        dataRowIdleData["SpeedAve"] = speedAverage;

                        speedMax = 0;
                        distanceCovered = 0;
                        longitudeFrom = longitudeTo;
                        latitudeFrom = latitudeTo;
                        geofenceFrom = geofenceTo;
                        dateTimeIdleFrom = dateTimeIdleTo;
                        dataTableIdleData.Rows.Add(dataRowIdleData);
                        idleStatusBefore = idleStatusNow;
                    }

                    if (no == dataTableHistoricalData.Rows.Count - 1) {
                        index++;
                        dateTimeIdleTo = dateTimeDateTo;//= (DateTime)dataTableHistoricalData.Rows[no]["DateTime"];
                        latitudeTo = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeTo = (double)dataTableHistoricalData.Rows[no]["Longitude"];
                        geofenceTo = (dataTableHistoricalData.Rows[no]["Geofence"] == System.DBNull.Value) ? "OUTSIDE" : (string)dataTableHistoricalData.Rows[no]["Geofence"];

                        timeSpan = dateTimeIdleTo - dateTimeIdleFrom;

                        speedAverage = speedAccumulator / 1;
                        speedAccumulator = 0;
                        speedDivisor = 0;


                        DataRow dataRowIdleData = dataTableIdleData.NewRow();
                        dataRowIdleData["No"] = index;
                        dataRowIdleData["Status"] = idleStatusNow;
                        dataRowIdleData["DateTimeFrom"] = dateTimeIdleFrom;
                        dataRowIdleData["DateTimeTo"] = dateTimeIdleTo;

                        dataRowIdleData["LatitudeFrom"] = latitudeFrom;
                        dataRowIdleData["LatitudeTo"] = latitudeTo;
                        dataRowIdleData["LongitudeFrom"] = longitudeFrom;
                        dataRowIdleData["LongitudeTo"] = longitudeTo;

                        dataRowIdleData["Time"] = timeSpan;
                        dataRowIdleData["Distance"] = distanceCovered;

                        if (geofenceTo.Equals(geofenceFrom)) {
                            dataRowIdleData["Geofences"] = geofenceFrom;
                        } else {
                            dataRowIdleData["Geofences"] = geofenceFrom + " to " + geofenceTo;
                        }

                        dataRowIdleData["Fuel"] = distanceCovered / Settings.Default.fuelLiterToKilometer;
                        dataRowIdleData["Cost"] = (double)dataRowIdleData["Fuel"] / Settings.Default.fuelLiterToCost;

                        dataRowIdleData["SpeedMax"] = speedMax;
                        dataRowIdleData["SpeedAve"] = speedAverage;

                        speedMax = 0;
                        distanceCovered = 0;
                        longitudeFrom = longitudeTo;
                        latitudeFrom = latitudeTo;
                        dateTimeIdleFrom = dateTimeIdleTo;
                        geofenceFrom = geofenceTo;
                        dataTableIdleData.Rows.Add(dataRowIdleData);
                        idleStatusBefore = idleStatusNow;
                    }

                    //Accumulators : SpeedAve, SpeedMax, Distance 
                    speedDivisor++;
                    speedAccumulator += speed;
                    if (speed > speedMax) {
                        speedMax = speed;
                    }
                    distanceCovered += distance;
                }


            } catch (QueryException queryException) {
                throw queryException;
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {

            }
            return dataTableIdleData;
        }

        public DataTable getTrackerGeofence(Company company, User user, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, int limit, int offset, Tracker tracker) {
            DataTable dataTableHistoricalData = this.getTrackerHistoricalData(company, user, dateTimeDateFrom, dateTimeDateTo, limit, offset, tracker);
            DataTable dataTableGeofenceData = this.getReportTable(ReportType.GEOFENCE);
            dataTableGeofenceData.TableName = tracker.DatabaseName;

            try {

                string geofenceStatusNow = "";
                string geofenceStatusBefore = "";

                bool acc = false;

                int index = 0;

                double speed = 0;
                double speedDivisor = 0;
                double speedAccumulator = 0;
                double speedMax = 0;
                double speedAverage = 0;

                double distance = 0;
                double distanceCovered = 0;

                DateTime dateTimeGeofenceFrom = new DateTime();
                DateTime dateTimeGeofenceTo = new DateTime();

                double latitudeTo = 0;
                double latitudeFrom = 0;
                double longitudeTo = 0;
                double longitudeFrom = 0;

                TimeSpan timeSpan;

                for (int no = 0; no < dataTableHistoricalData.Rows.Count; no++) {

                    acc = (bool)dataTableHistoricalData.Rows[no]["ACC"];
                    speed = (int)dataTableHistoricalData.Rows[no]["Speed"];
                    distance = (double)dataTableHistoricalData.Rows[no]["Distance"];

                    if (!(dataTableHistoricalData.Rows[no]["Geofence"] == System.DBNull.Value)) {
                        geofenceStatusNow = (string)dataTableHistoricalData.Rows[no]["Geofence"];
                    } else {
                        geofenceStatusNow = "-";
                    }

                    //OneTime
                    if ((int)dataTableHistoricalData.Rows[no]["No"] == 1) {
                        dateTimeGeofenceFrom = dateTimeDateFrom;//(DateTime)dataTableHistoricalData.Rows[no]["DateTime"];
                        latitudeFrom = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeFrom = (double)dataTableHistoricalData.Rows[no]["Longitude"];
                        geofenceStatusBefore = geofenceStatusNow;
                    }

                    if (geofenceStatusBefore != geofenceStatusNow) {
                        index++;

                        dateTimeGeofenceTo = (DateTime)dataTableHistoricalData.Rows[no]["DateTime"];
                        latitudeTo = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeTo = (double)dataTableHistoricalData.Rows[no]["Longitude"];

                        timeSpan = dateTimeGeofenceTo - dateTimeGeofenceFrom;

                        speedAverage = speedAccumulator / speedDivisor;
                        speedAccumulator = 0;
                        speedDivisor = 0;


                        DataRow dataRowGeofenceData = dataTableGeofenceData.NewRow();
                        dataRowGeofenceData["No"] = index;

                        dataRowGeofenceData["DateTimeFrom"] = dateTimeGeofenceFrom;
                        dataRowGeofenceData["DateTimeTo"] = dateTimeGeofenceTo;

                        dataRowGeofenceData["LatitudeFrom"] = latitudeFrom;
                        dataRowGeofenceData["LatitudeTo"] = latitudeTo;
                        dataRowGeofenceData["LongitudeFrom"] = longitudeFrom;
                        dataRowGeofenceData["LongitudeTo"] = longitudeTo;

                        dataRowGeofenceData["Time"] = timeSpan;
                        dataRowGeofenceData["Distance"] = distanceCovered;

                        dataRowGeofenceData["Fuel"] = distanceCovered / Settings.Default.fuelLiterToKilometer;
                        dataRowGeofenceData["Cost"] = ((double)dataRowGeofenceData["Fuel"]) / Settings.Default.fuelLiterToCost;


                        dataRowGeofenceData["Geofence"] = geofenceStatusBefore;
                        dataRowGeofenceData["SpeedMax"] = speedMax;
                        dataRowGeofenceData["SpeedAve"] = speedAverage;

                        dataRowGeofenceData["Status"] = (geofenceStatusBefore == "-" ? false : true);

                        speedMax = 0;
                        distanceCovered = 0;
                        dateTimeGeofenceFrom = dateTimeGeofenceTo;
                        geofenceStatusBefore = geofenceStatusNow;
                        longitudeFrom = longitudeTo;
                        latitudeFrom = latitudeTo;
                        dataTableGeofenceData.Rows.Add(dataRowGeofenceData);
                    }

                    if (no == dataTableHistoricalData.Rows.Count - 1) {
                        index++;

                        dateTimeGeofenceTo = dateTimeDateTo;//(DateTime)dataTableHistoricalData.Rows[no]["DateTime"];
                        latitudeTo = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeTo = (double)dataTableHistoricalData.Rows[no]["Longitude"];

                        timeSpan = dateTimeGeofenceTo - dateTimeGeofenceFrom;

                        speedAverage = speedAccumulator / 1;
                        speedAccumulator = 0;
                        speedDivisor = 0;


                        DataRow dataRowGeofenceData = dataTableGeofenceData.NewRow();
                        dataRowGeofenceData["No"] = index;

                        dataRowGeofenceData["DateTimeFrom"] = dateTimeGeofenceFrom;
                        dataRowGeofenceData["DateTimeTo"] = dateTimeGeofenceTo;

                        dataRowGeofenceData["LatitudeFrom"] = latitudeFrom;
                        dataRowGeofenceData["LatitudeTo"] = latitudeTo;
                        dataRowGeofenceData["LongitudeFrom"] = longitudeFrom;
                        dataRowGeofenceData["LongitudeTo"] = longitudeTo;

                        dataRowGeofenceData["Time"] = timeSpan;
                        dataRowGeofenceData["Distance"] = distanceCovered;



                        dataRowGeofenceData["Fuel"] = distanceCovered / Settings.Default.fuelLiterToKilometer;
                        dataRowGeofenceData["Cost"] = (double)dataRowGeofenceData["Fuel"] / Settings.Default.fuelLiterToCost;


                        dataRowGeofenceData["Geofence"] = geofenceStatusNow;
                        dataRowGeofenceData["SpeedMax"] = speedMax;
                        dataRowGeofenceData["SpeedAve"] = speedAverage;
                        dataRowGeofenceData["Status"] = (geofenceStatusBefore == "-" ? false : true);

                        speedMax = 0;
                        distanceCovered = 0;
                        dateTimeGeofenceFrom = dateTimeGeofenceTo;
                        geofenceStatusBefore = geofenceStatusNow;
                        longitudeFrom = longitudeTo;
                        latitudeFrom = latitudeTo;
                        dataTableGeofenceData.Rows.Add(dataRowGeofenceData);
                    }

                    //Accumulators : SpeedAve, SpeedMax, Distance 
                    speedDivisor++;
                    speedAccumulator += speed;
                    if (speed > speedMax) {
                        speedMax = speed;
                    }
                    distanceCovered += distance;

                }

                return dataTableGeofenceData;

            } catch (QueryException queryException) {
                throw queryException;
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            }
        }

        public DataTable getTrackerAccData(Company company, User user, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, int limit, int offset, Tracker tracker) {
            DataTable dataTableHistoricalData = this.getTrackerHistoricalData(company, user, dateTimeDateFrom, dateTimeDateTo, limit, offset, tracker);
            DataTable dataTableIAccData = this.getReportTable(ReportType.ACC);
            dataTableIAccData.TableName = tracker.DatabaseName;

            try {

                bool accStatusNow = false;
                bool accStatusBefore = false;


                int index = 0;

                double speed = 0;
                double speedDivisor = 0;
                double speedAccumulator = 0;
                double speedMax = 0;
                double speedAverage = 0;


                double distanceCovered = 0;
                double distance = 0;

                String geofenceFrom = "";
                String geofenceTo = "";

                DateTime dateTimeRunningFrom = new DateTime();
                DateTime dateTimeRunningTo = new DateTime();

                double latitudeTo = 0;
                double latitudeFrom = 0;
                double longitudeTo = 0;
                double longitudeFrom = 0;

                TimeSpan timeSpan;


                for (int no = 0; no < dataTableHistoricalData.Rows.Count; no++) {
                    //Initialized
                    accStatusNow = (bool)dataTableHistoricalData.Rows[no]["ACC"];
                    speed = (int)dataTableHistoricalData.Rows[no]["Speed"];
                    distance = (double)dataTableHistoricalData.Rows[no]["Distance"];

                    //OneTime
                    if ((int)dataTableHistoricalData.Rows[no]["No"] == 1) {
                        dateTimeRunningFrom = dateTimeDateFrom;//(DateTime)dataTableHistoricalData.Rows[no]["DateTime"];
                        latitudeFrom = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeFrom = (double)dataTableHistoricalData.Rows[no]["Longitude"];
                        geofenceFrom = (dataTableHistoricalData.Rows[no]["Geofence"] == System.DBNull.Value) ? "OUTSIDE" : (string)dataTableHistoricalData.Rows[no]["Geofence"];

                        accStatusBefore = accStatusNow;
                    }

                    if (accStatusBefore != accStatusNow) {
                        index++;
                        dateTimeRunningTo = (DateTime)dataTableHistoricalData.Rows[no]["DateTime"];
                        latitudeTo = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeTo = (double)dataTableHistoricalData.Rows[no]["Longitude"];
                        geofenceTo = (dataTableHistoricalData.Rows[no]["Geofence"] == System.DBNull.Value) ? "OUTSIDE" : (string)dataTableHistoricalData.Rows[no]["Geofence"];

                        timeSpan = dateTimeRunningTo - dateTimeRunningFrom;

                        speedAverage = speedAccumulator / 1;
                        speedAccumulator = 0;
                        speedDivisor = 0;


                        DataRow dataRowAccData = dataTableIAccData.NewRow();
                        dataRowAccData["No"] = index;
                        dataRowAccData["Status"] = accStatusBefore;
                        dataRowAccData["DateTimeFrom"] = dateTimeRunningFrom;
                        dataRowAccData["DateTimeTo"] = dateTimeRunningTo;

                        dataRowAccData["LatitudeFrom"] = latitudeFrom;
                        dataRowAccData["LatitudeTo"] = latitudeTo;
                        dataRowAccData["LongitudeFrom"] = longitudeFrom;
                        dataRowAccData["LongitudeTo"] = longitudeTo;

                        dataRowAccData["Time"] = timeSpan;
                        dataRowAccData["Distance"] = distanceCovered;

                        dataRowAccData["Fuel"] = distanceCovered / Settings.Default.fuelLiterToKilometer;
                        dataRowAccData["Cost"] = (double)dataRowAccData["Fuel"] / Settings.Default.fuelLiterToCost;

                        if (geofenceTo.Equals(geofenceFrom)) {
                            dataRowAccData["Geofences"] = geofenceFrom;
                        } else {
                            dataRowAccData["Geofences"] = geofenceFrom + " to " + geofenceTo;
                        }
                        dataRowAccData["SpeedMax"] = speedMax;
                        dataRowAccData["SpeedAve"] = speedAverage;

                        speedMax = 0;
                        distanceCovered = 0;
                        dateTimeRunningFrom = dateTimeRunningTo;
                        longitudeFrom = longitudeTo;
                        latitudeFrom = latitudeTo;
                        geofenceFrom = geofenceTo;

                        dataTableIAccData.Rows.Add(dataRowAccData);
                        accStatusBefore = accStatusNow;

                    }
                    if (no == dataTableHistoricalData.Rows.Count - 1) {
                        index++;
                        dateTimeRunningTo = dateTimeDateTo;//(DateTime)dataTableHistoricalData.Rows[no]["DateTime"];
                        latitudeTo = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeTo = (double)dataTableHistoricalData.Rows[no]["Longitude"];
                        geofenceTo = (dataTableHistoricalData.Rows[no]["Geofence"] == System.DBNull.Value) ? "OUTSIDE" : (string)dataTableHistoricalData.Rows[no]["Geofence"];

                        timeSpan = dateTimeRunningTo - dateTimeRunningFrom;

                        speedAverage = speedAccumulator / 1;
                        speedAccumulator = 0;
                        speedDivisor = 0;


                        DataRow dataRowAccData = dataTableIAccData.NewRow();
                        dataRowAccData["No"] = index;
                        dataRowAccData["Status"] = accStatusNow;

                        dataRowAccData["DateTimeFrom"] = dateTimeRunningFrom;
                        dataRowAccData["DateTimeTo"] = dateTimeRunningTo;

                        dataRowAccData["LatitudeFrom"] = latitudeFrom;
                        dataRowAccData["LatitudeTo"] = latitudeTo;
                        dataRowAccData["LongitudeFrom"] = longitudeFrom;
                        dataRowAccData["LongitudeTo"] = longitudeTo;

                        dataRowAccData["Time"] = timeSpan;
                        dataRowAccData["Distance"] = distanceCovered;

                        dataRowAccData["Fuel"] = distanceCovered / Settings.Default.fuelLiterToKilometer;
                        dataRowAccData["Cost"] = (double)dataRowAccData["Fuel"] / Settings.Default.fuelLiterToCost;

                        if (geofenceTo.Equals(geofenceFrom)) {
                            dataRowAccData["Geofences"] = geofenceFrom;
                        } else {
                            dataRowAccData["Geofences"] = geofenceFrom + " to " + geofenceTo;
                        }

                        dataRowAccData["SpeedMax"] = speedMax;
                        dataRowAccData["SpeedAve"] = speedAverage;

                        speedMax = 0;
                        distanceCovered = 0;
                        dateTimeRunningFrom = dateTimeRunningTo;
                        longitudeFrom = longitudeTo;
                        latitudeFrom = latitudeTo;
                        geofenceFrom = geofenceTo;


                        dataTableIAccData.Rows.Add(dataRowAccData);
                        accStatusBefore = accStatusNow;

                    }

                    //Accumulators : SpeedAve, SpeedMax, Distance 
                    speedDivisor++;
                    speedAccumulator += speed;
                    if (speed > speedMax) {
                        speedMax = speed;
                    }
                    distanceCovered += distance;
                }
            } catch (QueryException queryException) {
                throw queryException;
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {

            }
            return dataTableIAccData;
        }

        public DataTable getTrackerExternalPowerCutData(Company company, User user, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, int limit, int offset, Tracker tracker) {
            DataTable dataTableHistoricalData = this.getTrackerHistoricalData(company, user, dateTimeDateFrom, dateTimeDateTo, limit, offset, tracker);
            DataTable dataTableExternalPowerCutData = this.getReportTable(ReportType.EXTERNAL_POWER_CUT);
            dataTableExternalPowerCutData.TableName = tracker.DatabaseName;


            try {

                bool externalPowerStatusNow = false;
                bool externalPowerStatusBefore = false;

                EventCode eventCode = EventCode.TRACK_BY_TIME_INTERVAL;

                int index = 0;

                double speed = 0;
                double speedDivisor = 0;
                double speedAccumulator = 0;
                double speedMax = 0;
                double speedAverage = 0;


                double distanceCovered = 0;
                double distance = 0;

                String geofenceFrom = "";
                String geofenceTo = "";

                DateTime dateTimeRunningFrom = new DateTime();
                DateTime dateTimeRunningTo = new DateTime();

                double latitudeTo = 0;
                double latitudeFrom = 0;
                double longitudeTo = 0;
                double longitudeFrom = 0;

                TimeSpan timeSpan;


                for (int no = 0; no < dataTableHistoricalData.Rows.Count; no++) {
                    //Initialized
                    eventCode = (dataTableHistoricalData.Rows[no]["EventCode"] == System.DBNull.Value) ?
                       EventCode.TRACK_BY_TIME_INTERVAL : (EventCode)Enum.Parse(typeof(EventCode), (string)dataTableHistoricalData.Rows[no]["EventCode"], true);
                    speed = (int)dataTableHistoricalData.Rows[no]["Speed"];
                    distance = (double)dataTableHistoricalData.Rows[no]["Distance"];
                    double externalVolt = (double)dataTableHistoricalData.Rows[no]["ExternalVoltage"];

                    //Conditions
                    if (eventCode == EventCode.EXTERNAL_BATTERY_CUT || externalVolt < 12) {
                        externalPowerStatusNow = true;
                    }
                    if (eventCode == EventCode.EXTERNAL_BATTERY_ON || externalVolt >= 12) {
                        externalPowerStatusNow = false;
                    }

                    //OneTime
                    if ((int)dataTableHistoricalData.Rows[no]["No"] == 1) {
                        dateTimeRunningFrom = dateTimeDateFrom;//(DateTime)dataTableHistoricalData.Rows[no]["DateTime"];
                        latitudeFrom = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeFrom = (double)dataTableHistoricalData.Rows[no]["Longitude"];
                        geofenceFrom = (dataTableHistoricalData.Rows[no]["Geofence"] == System.DBNull.Value) ? "OUTSIDE" : (string)dataTableHistoricalData.Rows[no]["Geofence"];
                        externalPowerStatusBefore = externalPowerStatusNow;
                    }

                    if (externalPowerStatusBefore != externalPowerStatusNow) {

                        index++;
                        dateTimeRunningTo = (DateTime)dataTableHistoricalData.Rows[no]["DateTime"];
                        latitudeTo = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeTo = (double)dataTableHistoricalData.Rows[no]["Longitude"];
                        geofenceTo = (dataTableHistoricalData.Rows[no]["Geofence"] == System.DBNull.Value) ? "OUTSIDE" : (string)dataTableHistoricalData.Rows[no]["Geofence"];

                        timeSpan = dateTimeRunningTo - dateTimeRunningFrom;

                        speedAverage = speedAccumulator / speedDivisor;
                        speedAccumulator = 0;
                        speedDivisor = 0;

                        DataRow dataRowExternalPowerCutData = dataTableExternalPowerCutData.NewRow();
                        dataRowExternalPowerCutData["No"] = index;
                        dataRowExternalPowerCutData["Status"] = externalPowerStatusBefore;
                        dataRowExternalPowerCutData["DateTimeFrom"] = dateTimeRunningFrom;
                        dataRowExternalPowerCutData["DateTimeTo"] = dateTimeRunningTo;

                        dataRowExternalPowerCutData["LatitudeFrom"] = latitudeFrom;
                        dataRowExternalPowerCutData["LatitudeTo"] = latitudeTo;
                        dataRowExternalPowerCutData["LongitudeFrom"] = longitudeFrom;
                        dataRowExternalPowerCutData["LongitudeTo"] = longitudeTo;

                        dataRowExternalPowerCutData["Time"] = timeSpan;
                        dataRowExternalPowerCutData["Distance"] = distanceCovered;

                        dataRowExternalPowerCutData["Fuel"] = distanceCovered / Settings.Default.fuelLiterToKilometer;
                        dataRowExternalPowerCutData["Cost"] = ((double)dataRowExternalPowerCutData["Fuel"]) / Settings.Default.fuelLiterToCost;

                        if (geofenceTo.Equals(geofenceFrom)) {
                            dataRowExternalPowerCutData["Geofences"] = geofenceFrom;
                        } else {
                            dataRowExternalPowerCutData["Geofences"] = geofenceFrom + " to " + geofenceTo;
                        }
                        dataRowExternalPowerCutData["SpeedMax"] = speedMax;
                        dataRowExternalPowerCutData["SpeedAve"] = speedAverage;

                        speedMax = 0;
                        distanceCovered = 0;
                        dateTimeRunningFrom = dateTimeRunningTo;
                        longitudeFrom = longitudeTo;
                        latitudeFrom = latitudeTo;
                        dataTableExternalPowerCutData.Rows.Add(dataRowExternalPowerCutData);
                        externalPowerStatusBefore = externalPowerStatusNow;

                    }
                    if (no == dataTableHistoricalData.Rows.Count - 1) {
                        index++;
                        dateTimeRunningTo = dateTimeDateTo;// (DateTime)dataTableHistoricalData.Rows[no]["DateTime"];
                        latitudeTo = (double)dataTableHistoricalData.Rows[no]["Latitude"];
                        longitudeTo = (double)dataTableHistoricalData.Rows[no]["Longitude"];
                        geofenceTo = (dataTableHistoricalData.Rows[no]["Geofence"] == System.DBNull.Value) ? "OUTSIDE" : (string)dataTableHistoricalData.Rows[no]["Geofence"];

                        timeSpan = dateTimeRunningTo - dateTimeRunningFrom;

                        speedAverage = speedAccumulator / 1;
                        speedAccumulator = 0;
                        speedDivisor = 0;


                        DataRow dataRowExternalPowerCutData = dataTableExternalPowerCutData.NewRow();
                        dataRowExternalPowerCutData["No"] = index;
                        dataRowExternalPowerCutData["Status"] = externalPowerStatusNow;
                        dataRowExternalPowerCutData["DateTimeFrom"] = dateTimeRunningFrom;
                        dataRowExternalPowerCutData["DateTimeTo"] = dateTimeRunningTo;

                        dataRowExternalPowerCutData["LatitudeFrom"] = latitudeFrom;
                        dataRowExternalPowerCutData["LatitudeTo"] = latitudeTo;
                        dataRowExternalPowerCutData["LongitudeFrom"] = longitudeFrom;
                        dataRowExternalPowerCutData["LongitudeTo"] = longitudeTo;

                        dataRowExternalPowerCutData["Time"] = timeSpan;
                        dataRowExternalPowerCutData["Distance"] = distanceCovered;

                        dataRowExternalPowerCutData["Fuel"] = distanceCovered / Settings.Default.fuelLiterToKilometer;
                        dataRowExternalPowerCutData["Cost"] = (double)dataRowExternalPowerCutData["Fuel"] / Settings.Default.fuelLiterToCost;

                        if (geofenceTo.Equals(geofenceFrom)) {
                            dataRowExternalPowerCutData["Geofences"] = geofenceFrom;
                        } else {
                            dataRowExternalPowerCutData["Geofences"] = geofenceFrom + " to " + geofenceTo;
                        }
                        dataRowExternalPowerCutData["SpeedMax"] = speedMax;
                        dataRowExternalPowerCutData["SpeedAve"] = speedAverage;

                        speedMax = 0;
                        distanceCovered = 0;
                        dateTimeRunningFrom = dateTimeRunningTo;
                        longitudeFrom = longitudeTo;
                        latitudeFrom = latitudeTo;

                        dataTableExternalPowerCutData.Rows.Add(dataRowExternalPowerCutData);
                        externalPowerStatusBefore = externalPowerStatusNow;

                    }
                    //Accumulators : SpeedAve, SpeedMax, Distance 
                    speedDivisor++;
                    speedAccumulator += speed;
                    if (speed > speedMax) {
                        speedMax = speed;
                    }
                    distanceCovered += distance;
                }
            } catch (QueryException queryException) {
                throw queryException;
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {

            }
            return dataTableExternalPowerCutData;
        }

        public DataTable getTrackerOverSpeedData(Company company, User user, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, int limit, int offset, Tracker tracker) {
            DataTable dataTableHistoricalData = this.getTrackerHistoricalData(company, user, dateTimeDateFrom, dateTimeDateTo, limit, offset, tracker);
            DataTable dataTableOverSpeedData = this.getReportTable(ReportType.OVERSPEED);
            dataTableOverSpeedData.TableName = tracker.DatabaseName;

            try {

                bool overSpeedStatusNow = false;
                int index = 0;
                String geofence = "";


                foreach (DataRow dataRowNow in dataTableHistoricalData.Rows) {

                    overSpeedStatusNow = (bool)dataRowNow["OverSpeed"];

                    if (overSpeedStatusNow) {

                        index++;

                        geofence = ((dataRowNow["Geofence"] == System.DBNull.Value) ? "" : (string)dataRowNow["Geofence"]);


                        DataRow dataRowOverspeedData = dataTableOverSpeedData.NewRow();
                        dataRowOverspeedData["No"] = index;
                        dataRowOverspeedData["Status"] = overSpeedStatusNow;
                        dataRowOverspeedData["DateTime"] = (DateTime)dataRowNow["DateTime"];
                        dataRowOverspeedData["Latitude"] = (double)dataRowNow["Latitude"];
                        dataRowOverspeedData["Longitude"] = (double)dataRowNow["Longitude"];
                        dataRowOverspeedData["Speed"] = (int)dataRowNow["Speed"];
                        dataRowOverspeedData["Mileage"] = (double)dataRowNow["Mileage"];
                        dataRowOverspeedData["Geofence"] = geofence;
                        dataTableOverSpeedData.Rows.Add(dataRowOverspeedData);
                    }
                }
            } catch (QueryException queryException) {
                throw queryException;
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {

            }
            return dataTableOverSpeedData;
        }

        public DataTable getTrackersGeofence(Company company, User user, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, int limit, int offset, List<Tracker> trackerList) {
            DataTable dataTable = this.getReportTable(ReportType.TRACKERS_GEOFENCE);
            //dataTable.TableName = "";
            try {
                int index = 0;
                foreach (Tracker tracker in trackerList) {
                    index++;

                    DataTable dataTableGeofence = (this.getTrackerGeofence(company, user, dateTimeDateFrom, dateTimeDateTo, limit, offset, tracker));

                    if (dataTableGeofence.Rows.Count <= 0) {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["No"] = index;
                        dataRow["VehicleRegistration"] = tracker.VehicleRegistration;
                        dataRow["DriverName"] = tracker.DriverName;
                        dataRow["DeviceImei"] = tracker.TrackerImei;

                        dataRow["DateTimeFrom"] = dateTimeDateFrom;
                        dataRow["DateTimeTo"] = dateTimeDateTo;
                        dataRow["SpeedAve"] = 0;
                        dataRow["SpeedMax"] = 0;
                        dataRow["Distance"] = 0;
                        dataRow["Fuel"] = 0;
                        dataRow["Cost"] = 0;
                        dataRow["Geofence"] = "";
                        dataTable.Rows.Add(dataRow);
                    } else {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["No"] = index;
                        dataRow["VehicleRegistration"] = tracker.VehicleRegistration;
                        dataRow["DriverName"] = tracker.DriverName;
                        dataRow["DeviceImei"] = tracker.TrackerImei;

                        dataRow["DateTimeFrom"] = dateTimeDateFrom;
                        dataRow["DateTimeTo"] = dateTimeDateTo;

                        double speedAve = 0;
                        if (dataTableGeofence.Rows.Count > 0)
                            speedAve = Converter.dataTableColumnSumValue(dataTableGeofence, dataTableGeofence.Columns["SpeedAve"]) / dataTableGeofence.Rows.Count;
                        dataRow["SpeedAve"] = speedAve;

                        double speedMax = Converter.dataTableColumnMaxValue(dataTableGeofence, dataTableGeofence.Columns["SpeedMax"]);
                        dataRow["SpeedMax"] = speedMax;

                        dataRow["Distance"] = Converter.dataTableColumnSumValue(dataTableGeofence, dataTableGeofence.Columns["Distance"]);
                        dataRow["Fuel"] = Converter.dataTableColumnSumValue(dataTableGeofence, dataTableGeofence.Columns["Fuel"]);
                        dataRow["Cost"] = Converter.dataTableColumnSumValue(dataTableGeofence, dataTableGeofence.Columns["Cost"]);

                        dataRow["Geofence"] = Converter.dataTableColumnAppend(dataTableGeofence, dataTableGeofence.Columns["Geofence"]);
                        dataTable.Rows.Add(dataRow);
                    }
                }
                return dataTable;
            } catch (QueryException queryException) {
                throw queryException;
            } catch (MySqlException mySqlException) {
                throw new QueryException(1, mySqlException.Message);
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            }
        }

        public DataTable getTrackersHistorical(Company company, User user, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, int limit, int offset, List<Tracker> trackerList) {
            DataTable dataTable = this.getReportTable(ReportType.TRACKERS_HISTORICAL);

            try {
                int index = 0;
                foreach (Tracker tracker in trackerList) {
                    index++;

                    DataTable dataTableHistorical = (this.getTrackerHistoricalData(company, user, dateTimeDateFrom, dateTimeDateTo, limit, offset, tracker));

                    if (dataTableHistorical.Rows.Count <= 0) {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["VehicleRegistration"] = tracker.VehicleRegistration;
                        dataRow["DriverName"] = tracker.DriverName;
                        dataRow["VehicleModel"] = tracker.VehicleModel;
                        dataRow["OwnerName"] = tracker.OwnerName;
                        dataRow["SimNumber"] = tracker.SimNumber;
                        dataRow["SimImei"] = tracker.SimImei;
                        dataRow["TrackerImei"] = tracker.TrackerImei;

                        dataTable.Rows.Add(dataRow);

                        DataRow dataRowEmpty = dataTable.NewRow();
                        dataTable.Rows.Add(dataRowEmpty);

                    } else {
                        foreach (DataRow dataRowItem in dataTableHistorical.Rows) {

                            DataRow dataRow = dataTable.NewRow();
                            dataRow["VehicleRegistration"] = tracker.VehicleRegistration;
                            dataRow["DriverName"] = tracker.DriverName;
                            dataRow["VehicleModel"] = tracker.VehicleModel;
                            dataRow["OwnerName"] = tracker.OwnerName;
                            dataRow["SimNumber"] = tracker.SimNumber;
                            dataRow["SimImei"] = tracker.SimImei;
                            dataRow["TrackerImei"] = tracker.TrackerImei;

                            dataRow["No"] = dataRowItem["No"];
                            dataRow["DateTime"] = dataRowItem["DateTime"];
                            dataRow["Latitude"] = dataRowItem["Latitude"];
                            dataRow["Longitude"] = dataRowItem["Longitude"];
                            dataRow["Speed"] = dataRowItem["Speed"];
                            dataRow["Mileage"] = dataRowItem["Mileage"];
                            dataRow["Distance"] = dataRowItem["Distance"];
                            dataRow["Fuel"] = dataRowItem["Fuel"];
                            dataRow["Cost"] = dataRowItem["Cost"];
                            dataRow["Altitude"] = dataRowItem["Altitude"];
                            dataRow["Degrees"] = dataRowItem["Degrees"];
                            dataRow["Direction"] = dataRowItem["Direction"];
                            dataRow["No"] = dataRowItem["No"];
                            dataRow["GpsSatellites"] = dataRowItem["GpsSatellites"];
                            dataRow["Latitude"] = dataRowItem["Latitude"];
                            dataRow["GsmSignal"] = dataRowItem["GsmSignal"];
                            dataRow["EventCode"] = dataRowItem["EventCode"];
                            dataRow["Geofence"] = dataRowItem["Geofence"];
                            dataRow["ACC"] = dataRowItem["ACC"];
                            dataRow["SOS"] = dataRowItem["SOS"];
                            dataRow["OverSpeed"] = dataRowItem["OverSpeed"];
                            dataRow["Battery"] = dataRowItem["Battery"];
                            dataRow["BatteryVoltage"] = dataRowItem["BatteryVoltage"];
                            dataRow["ExternalVoltage"] = dataRowItem["ExternalVoltage"];

                            dataTable.Rows.Add(dataRow);
                        }
                        DataRow dataRowEmpty = dataTable.NewRow();
                        dataTable.Rows.Add(dataRowEmpty);
                    }
                }
                return dataTable;
            } catch (QueryException queryException) {
                throw queryException;
            } catch (MySqlException mySqlException) {
                throw new QueryException(1, mySqlException.Message);
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            }
        }


        public DataTable getReportTable(ReportType reportType) {
            DataTable dataTable = new DataTable();
            switch (reportType) {
                case ReportType.HISTORICAL:
                    dataTable.Columns.Add("No", typeof(int));
                    dataTable.Columns.Add("DateTime", typeof(DateTime));
                    dataTable.Columns.Add("Latitude", typeof(double));
                    dataTable.Columns.Add("Longitude", typeof(double));
                    dataTable.Columns.Add("Speed", typeof(int));
                    dataTable.Columns.Add("Mileage", typeof(double));
                    dataTable.Columns.Add("Distance", typeof(double));
                    dataTable.Columns.Add("Fuel", typeof(double));
                    dataTable.Columns.Add("Cost", typeof(double));
                    dataTable.Columns.Add("Altitude", typeof(int));
                    dataTable.Columns.Add("Degrees", typeof(int));
                    dataTable.Columns.Add("Direction", typeof(string));
                    dataTable.Columns.Add("GpsSatellites", typeof(int));
                    dataTable.Columns.Add("GsmSignal", typeof(int));
                    dataTable.Columns.Add("EventCode", typeof(string));
                    dataTable.Columns.Add("Geofence", typeof(string));
                    dataTable.Columns.Add("ACC", typeof(bool));
                    dataTable.Columns.Add("SOS", typeof(bool));
                    dataTable.Columns.Add("OverSpeed", typeof(bool));
                    dataTable.Columns.Add("Battery", typeof(double));
                    dataTable.Columns.Add("BatteryVoltage", typeof(double));
                    dataTable.Columns.Add("ExternalVoltage", typeof(double));
                    break;
                case ReportType.RUNNING:
                case ReportType.IDLING:
                case ReportType.ACC:
                case ReportType.EXTERNAL_POWER_CUT:
                    dataTable.Columns.Add("No", typeof(int));
                    dataTable.Columns.Add("Status", typeof(bool));
                    dataTable.Columns.Add("DateTimeFrom", typeof(DateTime));
                    dataTable.Columns.Add("DateTimeTo", typeof(DateTime));

                    dataTable.Columns.Add("LatitudeFrom", typeof(double));
                    dataTable.Columns.Add("LongitudeFrom", typeof(double));
                    dataTable.Columns.Add("LatitudeTo", typeof(double));
                    dataTable.Columns.Add("LongitudeTo", typeof(double));

                    dataTable.Columns.Add("Time", typeof(TimeSpan));
                    dataTable.Columns.Add("SpeedMax", typeof(double));
                    dataTable.Columns.Add("SpeedAve", typeof(double));
                    dataTable.Columns.Add("Distance", typeof(double));
                    dataTable.Columns.Add("Fuel", typeof(double));
                    dataTable.Columns.Add("Cost", typeof(double));
                    dataTable.Columns.Add("Geofences", typeof(string));
                    break;
                case ReportType.GEOFENCE:
                    dataTable.Columns.Add("No", typeof(int));
                    dataTable.Columns.Add("Status", typeof(bool));
                    dataTable.Columns.Add("DateTimeFrom", typeof(DateTime));
                    dataTable.Columns.Add("DateTimeTo", typeof(DateTime));

                    dataTable.Columns.Add("LatitudeFrom", typeof(double));
                    dataTable.Columns.Add("LongitudeFrom", typeof(double));
                    dataTable.Columns.Add("LatitudeTo", typeof(double));
                    dataTable.Columns.Add("LongitudeTo", typeof(double));


                    dataTable.Columns.Add("Time", typeof(TimeSpan));
                    dataTable.Columns.Add("SpeedMax", typeof(double));
                    dataTable.Columns.Add("SpeedAve", typeof(double));
                    dataTable.Columns.Add("Distance", typeof(double));
                    dataTable.Columns.Add("Fuel", typeof(double));
                    dataTable.Columns.Add("Cost", typeof(double));
                    dataTable.Columns.Add("Geofence", typeof(string));
                    break;
                case ReportType.OVERSPEED:
                    dataTable.Columns.Add("No", typeof(int));
                    dataTable.Columns.Add("Status", typeof(bool));
                    dataTable.Columns.Add("DateTime", typeof(DateTime));
                    dataTable.Columns.Add("Latitude", typeof(double));
                    dataTable.Columns.Add("Longitude", typeof(double));
                    dataTable.Columns.Add("Speed", typeof(int));
                    dataTable.Columns.Add("Mileage", typeof(double));
                    dataTable.Columns.Add("Geofence", typeof(string));
                    break;
                case ReportType.TRACKERS_GEOFENCE:
                    dataTable.Columns.Add("No", typeof(int));
                    dataTable.Columns.Add("VehicleRegistration", typeof(string));
                    dataTable.Columns.Add("DriverName", typeof(string));
                    dataTable.Columns.Add("DeviceImei", typeof(string));
                    dataTable.Columns.Add("DateTimeFrom", typeof(DateTime));
                    dataTable.Columns.Add("DateTimeTo", typeof(DateTime));
                    dataTable.Columns.Add("SpeedMax", typeof(double));
                    dataTable.Columns.Add("SpeedAve", typeof(double));
                    dataTable.Columns.Add("Distance", typeof(double));
                    dataTable.Columns.Add("Fuel", typeof(double));
                    dataTable.Columns.Add("Cost", typeof(double));
                    dataTable.Columns.Add("Geofence", typeof(string));
                    break;
                case ReportType.TRACKERS_HISTORICAL:
                    dataTable.Columns.Add("VehicleRegistration", typeof(string));
                    dataTable.Columns.Add("DriverName", typeof(string));
                    dataTable.Columns.Add("VehicleModel", typeof(string));
                    dataTable.Columns.Add("OwnerName", typeof(string));
                    dataTable.Columns.Add("SimNumber", typeof(string));
                    dataTable.Columns.Add("SimImei", typeof(string));
                    dataTable.Columns.Add("TrackerImei", typeof(string));

                    dataTable.Columns.Add("No", typeof(int));
                    dataTable.Columns.Add("DateTime", typeof(DateTime));
                    dataTable.Columns.Add("Latitude", typeof(double));
                    dataTable.Columns.Add("Longitude", typeof(double));
                    dataTable.Columns.Add("Speed", typeof(int));
                    dataTable.Columns.Add("Mileage", typeof(double));
                    dataTable.Columns.Add("Distance", typeof(double));
                    dataTable.Columns.Add("Fuel", typeof(double));
                    dataTable.Columns.Add("Cost", typeof(double));
                    dataTable.Columns.Add("Altitude", typeof(int));
                    dataTable.Columns.Add("Degrees", typeof(int));
                    dataTable.Columns.Add("Direction", typeof(string));
                    dataTable.Columns.Add("GpsSatellites", typeof(int));
                    dataTable.Columns.Add("GsmSignal", typeof(int));
                    dataTable.Columns.Add("EventCode", typeof(string));
                    dataTable.Columns.Add("Geofence", typeof(string));
                    dataTable.Columns.Add("ACC", typeof(bool));
                    dataTable.Columns.Add("SOS", typeof(bool));
                    dataTable.Columns.Add("OverSpeed", typeof(bool));
                    dataTable.Columns.Add("Battery", typeof(double));
                    dataTable.Columns.Add("BatteryVoltage", typeof(double));
                    dataTable.Columns.Add("ExternalVoltage", typeof(double));
                    break;
                case ReportType.TRACKERS:
                case ReportType.ALL_TRACKERS:
                    dataTable.Columns.Add("id", typeof(int));

                    dataTable.Columns.Add("vehicleRegistration", typeof(string));
                    dataTable.Columns.Add("vehicleModel", typeof(string));
                    dataTable.Columns.Add("ownerName", typeof(string));
                    dataTable.Columns.Add("driverName", typeof(string));

                    dataTable.Columns.Add("simImei", typeof(string));
                    dataTable.Columns.Add("simNumber", typeof(string));
                    dataTable.Columns.Add("mobileDataProvider", typeof(int));

                    dataTable.Columns.Add("deviceImei", typeof(string));
                    dataTable.Columns.Add("devicePassword", typeof(string));
                    dataTable.Columns.Add("deviceType", typeof(int));

                    dataTable.Columns.Add("emails", typeof(string));
                    dataTable.Columns.Add("users", typeof(string));

                    dataTable.Columns.Add("mileageInitial", typeof(int));
                    dataTable.Columns.Add("mileageLimit", typeof(int));
                    dataTable.Columns.Add("SpeedLimit", typeof(int));

                    dataTable.Columns.Add("idlingTime", typeof(int));
                    dataTable.Columns.Add("inputs", typeof(string));
                    dataTable.Columns.Add("imageNumber", typeof(int));
                    dataTable.Columns.Add("note", typeof(string));

                    dataTable.Columns.Add("collections", typeof(string));
                    dataTable.Columns.Add("companyDatabaseName", typeof(string));
                    dataTable.Columns.Add("databaseHost", typeof(int));
                    dataTable.Columns.Add("DatabaseName", typeof(string));
                    dataTable.Columns.Add("httpHost", typeof(int));


                    dataTable.Columns.Add("dateCreated", typeof(DateTime));
                    dataTable.Columns.Add("dateExpired", typeof(DateTime));
                    break;
                case ReportType.ALL_COMPANIES:
                    dataTable.Columns.Add("id", typeof(int));
                    dataTable.Columns.Add("host", typeof(string));
                    dataTable.Columns.Add("username", typeof(string));
                    dataTable.Columns.Add("displayName", typeof(string));
                    dataTable.Columns.Add("email", typeof(string));
                    dataTable.Columns.Add("address", typeof(string));
                    dataTable.Columns.Add("telephoneNumber", typeof(string));
                    dataTable.Columns.Add("mobileNumber", typeof(string));

                    dataTable.Columns.Add("databaseName", typeof(string));
                    dataTable.Columns.Add("isActive", typeof(bool));

                    dataTable.Columns.Add("dateCreated", typeof(DateTime));
                    dataTable.Columns.Add("dateExpired", typeof(DateTime));
                    break;
            }
            return dataTable;
        }
    }

}



