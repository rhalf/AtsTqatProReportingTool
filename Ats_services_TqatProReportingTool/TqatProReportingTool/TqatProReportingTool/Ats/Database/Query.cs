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
using Ats.Map;

namespace Ats.Database {



    class Query {

        MySqlConnection mysqlConnection = null;
        Database database;
        String sql;
        Account account;

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

        public void checkLogin(Account account) {
            this.account = account;

            try {
                //1st Query
                //checking if the company is existing
                mysqlConnection.Open();

                sql =
                    "SELECT * " +
                    "FROM dbt_tracking_master.cmps " +
                    "WHERE dbt_tracking_master.cmps.cmpname = @sCompanyName;";

                MySqlCommand mySqlCommand = new MySqlCommand(this.sql, mysqlConnection);
                mySqlCommand.Parameters.AddWithValue("@sCompanyName", this.account.companyUsername);

                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();



                if (!mySqlDataReader.HasRows) {
                    mySqlDataReader.Dispose();
                    throw new QueryException(1, "Company does not exist.");
                } else {
                    mySqlDataReader.Read();
                    this.account.companyUsername = mySqlDataReader.GetString("cmpname");
                    this.account.databaseName = "cmp_" + mySqlDataReader.GetString("cmpdbname");
                    this.account.companyDisplayName = mySqlDataReader.GetString("cmpdisplayname");

                    mysqlConnection.Close();

                    //2nd Query
                    //checking if user is existing in the company
                    mysqlConnection = new MySqlConnection(database.getConnectionString());

                    mysqlConnection.Open();

                    sql =
                        "SELECT * " +
                        "FROM " + this.account.databaseName + ".usrs " +
                        "WHERE " +
                        this.account.databaseName + ".usrs.uname = @sUsername AND " +
                        this.account.databaseName + ".usrs.upass = @sPassword;";
                    mySqlCommand = new MySqlCommand(sql, mysqlConnection);



                    mySqlCommand.Parameters.AddWithValue("@sUsername", account.username);
                    mySqlCommand.Parameters.AddWithValue("@sPassword", account.password);

                    mySqlDataReader = mySqlCommand.ExecuteReader();

                    if (!mySqlDataReader.HasRows) {
                        mySqlDataReader.Dispose();
                        throw new QueryException(1, "Username or Password does not exist.");
                    } else {
                        mySqlDataReader.Read();
                        this.account.accessLevel = int.Parse(mySqlDataReader.GetString("upriv"));
                        this.account.username = mySqlDataReader.GetString("uname");
                        this.account.password = mySqlDataReader.GetString("upass");
                        this.account.id = int.Parse(mySqlDataReader.GetString("uid"));
                        this.account.active = int.Parse(mySqlDataReader.GetString("uactive"));

                        if (!String.IsNullOrEmpty(mySqlDataReader.GetString("uexpiredate"))) {
                            string dateTime = (mySqlDataReader.GetString("uexpiredate"));
                            if (!String.IsNullOrEmpty(dateTime)) {
                                DateTime parsedDate = Converter.subStandardDateTimeToDateTime(dateTime);
                                //= DateTime.ParseExact(dateTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                                this.account.dateTimeExpired = parsedDate;
                            }
                        } else {
                            this.account.dateTimeExpired = new DateTime(2050, 01, 01);
                        }

                        if (!String.IsNullOrEmpty(mySqlDataReader.GetString("ucreatedate"))) {
                            string dateTime = mySqlDataReader.GetString("ucreatedate");
                            if (!String.IsNullOrEmpty(dateTime)) {
                                DateTime parsedDate = Converter.subStandardDateTimeToDateTime(dateTime);
                                //DateTime parsedDate = DateTime.ParseExact(dateTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                this.account.dateTimeCreated = parsedDate;
                            }
                        } else {
                            this.account.dateTimeCreated = new DateTime(2010, 01, 01);
                            mySqlDataReader.Dispose();
                        }
                        mySqlDataReader.Dispose();

                        //3rd query
                        //Getting all the geofences
                        mysqlConnection = new MySqlConnection(database.getConnectionString());

                        mysqlConnection.Open();

                        sql =
                            "SELECT * " +
                            "FROM " + this.account.databaseName + ".gf";

                        mySqlCommand = new MySqlCommand(sql, mysqlConnection);
                        mySqlDataReader = mySqlCommand.ExecuteReader();

                        if (!mySqlDataReader.HasRows) {
                            mySqlDataReader.Dispose();
                        } else {
                            while (mySqlDataReader.Read()) {
                                string geofenceData = (string)mySqlDataReader["gf_data"];
                                string geofenceName = (string)mySqlDataReader["gf_name"];

                                geofenceData = geofenceData.Replace("),( ", "|");
                                geofenceData = geofenceData.Replace(")", string.Empty);
                                geofenceData = geofenceData.Replace("(", string.Empty);
                                geofenceData = geofenceData.Replace(" ", string.Empty);

                                Geofence geofence = new Geofence();
                                List<string> points = geofenceData.Split('|').ToList();
                                foreach (string point in points) {
                                    string[] coordinates = point.Split(',');
                                    double latitude = double.Parse(coordinates[0]);
                                    double longitude = double.Parse(coordinates[1]);
                                    Location location = new Location(latitude, longitude);
                                    geofence.addPoint(location);
                                }

                                geofence.Name = geofenceName;
                                account.geofenceCollection.Add(geofence);
                            }

                            mySqlDataReader.Dispose();
                        }

                    }

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

        public DataTable getAllCompanies() {
            DataTable dataTable = new DataTable();
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
                    dataTable.Load(mySqlDataReader);
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
            DataTable dataTable = new DataTable();
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
                    dataTable.Columns.Add("speedLimit", typeof(int));

                    dataTable.Columns.Add("idlingTime", typeof(int));
                    dataTable.Columns.Add("inputs", typeof(string));
                    dataTable.Columns.Add("imageNumber", typeof(int));
                    dataTable.Columns.Add("note", typeof(string));

                    dataTable.Columns.Add("collections", typeof(string));
                    dataTable.Columns.Add("companyDatabaseName", typeof(string));
                    dataTable.Columns.Add("databaseHost", typeof(int));
                    dataTable.Columns.Add("dataDatabaseName", typeof(string));
                    dataTable.Columns.Add("httpHost", typeof(int));


                    dataTable.Columns.Add("dateCreated", typeof(DateTime));
                    dataTable.Columns.Add("dateExpired", typeof(DateTime));

                    Tracker tracker = new Tracker();
                    string dateTime;

                    while (mySqlDataReader.Read()) {

                        tracker.collections = (string)mySqlDataReader["tcollections"];
                        tracker.companyDatabaseName = (string)mySqlDataReader["tcmp"];
                        tracker.databaseHost = int.Parse((string)mySqlDataReader["tdbhost"]);
                        tracker.dataDatabaseName = (string)mySqlDataReader["tdbs"];

                        dateTime = (string)mySqlDataReader["tcreatedate"];
                        tracker.dateCreated = Converter.subStandardDateTimeToDateTime(dateTime);
                        dateTime = String.Empty;

                        dateTime = (string)mySqlDataReader["ttrackerexpiry"];
                        tracker.dateExpired = Converter.subStandardDateTimeToDateTime(dateTime);
                        dateTime = String.Empty;

                        tracker.deviceImei = (string)mySqlDataReader["tunit"];
                        tracker.devicePassword = (string)mySqlDataReader["tunitpassword"];
                        tracker.deviceType = int.Parse((string)mySqlDataReader["ttype"]);
                        tracker.driverName = (string)mySqlDataReader["tdrivername"];
                        tracker.emails = (string)mySqlDataReader["temails"];
                        tracker.httpHost = int.Parse((string)mySqlDataReader["thttphost"]);
                        tracker.id = (int)mySqlDataReader["tid"];
                        tracker.idlingTime = int.Parse((string)mySqlDataReader["tidlingtime"]);
                        tracker.imageNumber = int.Parse((string)mySqlDataReader["timg"]);
                        tracker.inputs = (string)mySqlDataReader["tinputs"];
                        tracker.mileageInitial = int.Parse((string)mySqlDataReader["tmileageInit"]);
                        tracker.mileageLimit = int.Parse((string)mySqlDataReader["tmileagelimit"]);
                        tracker.mobileDataProvider = int.Parse((string)mySqlDataReader["tprovider"]);
                        tracker.note = (string)mySqlDataReader["tnote"];
                        tracker.ownerName = (string)mySqlDataReader["townername"];
                        tracker.simImei = (string)mySqlDataReader["tsimsr"];
                        tracker.simNumber = (string)mySqlDataReader["tsimno"];
                        tracker.users = (string)mySqlDataReader["tusers"];
                        tracker.speedLimit = int.Parse((string)mySqlDataReader["tspeedlimit"]);
                        tracker.vehicleModel = (string)mySqlDataReader["tvehiclemodel"];
                        tracker.vehicleRegistration = (string)mySqlDataReader["tvehiclereg"];


                        DataRow dataRow = dataTable.NewRow();
                        dataRow["id"] = tracker.id;
                        dataRow["vehicleRegistration"] = tracker.vehicleRegistration;
                        dataRow["vehicleModel"] = tracker.vehicleModel;
                        dataRow["ownerName"] = tracker.ownerName;
                        dataRow["driverName"] = tracker.driverName;

                        dataRow["simImei"] = tracker.simImei;
                        dataRow["simNumber"] = tracker.simNumber;
                        dataRow["mobileDataProvider"] = tracker.mobileDataProvider;

                        dataRow["deviceImei"] = tracker.deviceImei;
                        dataRow["devicePassword"] = tracker.devicePassword;
                        dataRow["deviceType"] = tracker.deviceType;

                        dataRow["emails"] = tracker.emails;
                        dataRow["users"] = tracker.users;

                        dataRow["mileageInitial"] = tracker.mileageInitial;
                        dataRow["mileageLimit"] = tracker.mileageLimit;
                        dataRow["speedLimit"] = tracker.speedLimit;

                        dataRow["idlingTime"] = tracker.idlingTime;
                        dataRow["inputs"] = tracker.inputs;
                        dataRow["imageNumber"] = tracker.imageNumber;
                        dataRow["note"] = tracker.note;

                        dataRow["collections"] = tracker.collections;
                        dataRow["companyDatabaseName"] = tracker.companyDatabaseName;
                        dataRow["databaseHost"] = tracker.databaseHost;
                        dataRow["dataDatabaseName"] = tracker.dataDatabaseName;
                        dataRow["httpHost"] = tracker.httpHost;
                        dataRow["dateCreated"] = tracker.dateCreated;
                        dataRow["dateExpired"] = tracker.dateExpired;

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

        public DataTable getTrackers(Account account, int userAccountId) {
            DataTable dataTable = new DataTable();
            try {
                mysqlConnection.Open();

                string sql =
                    "SELECT * " +
                    "FROM dbt_tracking_master.trks " +
                    "WHERE dbt_tracking_master.trks.tcmp = @sCompanyName " +
                    "AND dbt_tracking_master.trks.tusers LIKE @sUserId;";

                MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);

                mySqlCommand.Parameters.AddWithValue("@sCompanyName", account.databaseName.Substring(4));
                mySqlCommand.Parameters.AddWithValue("@sUserId", "%" + userAccountId.ToString() + "%");

                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                if (!mySqlDataReader.HasRows) {
                    throw new QueryException(1, "Tracker's Collection is empty.");
                } else {
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
                    dataTable.Columns.Add("speedLimit", typeof(int));

                    dataTable.Columns.Add("idlingTime", typeof(int));
                    dataTable.Columns.Add("inputs", typeof(string));
                    dataTable.Columns.Add("imageNumber", typeof(int));
                    dataTable.Columns.Add("note", typeof(string));

                    dataTable.Columns.Add("collections", typeof(string));
                    dataTable.Columns.Add("companyDatabaseName", typeof(string));
                    dataTable.Columns.Add("databaseHost", typeof(int));
                    dataTable.Columns.Add("dataDatabaseName", typeof(string));
                    dataTable.Columns.Add("httpHost", typeof(int));


                    dataTable.Columns.Add("dateCreated", typeof(DateTime));
                    dataTable.Columns.Add("dateExpired", typeof(DateTime));

                    Tracker tracker = new Tracker();
                    string dateTime;

                    while (mySqlDataReader.Read()) {

                        tracker.collections = (string)mySqlDataReader["tcollections"];
                        tracker.companyDatabaseName = (string)mySqlDataReader["tcmp"];
                        tracker.databaseHost = int.Parse((string)mySqlDataReader["tdbhost"]);
                        tracker.dataDatabaseName = (string)mySqlDataReader["tdbs"];

                        dateTime = (string)mySqlDataReader["tcreatedate"];
                        tracker.dateCreated = Converter.subStandardDateTimeToDateTime(dateTime);
                        dateTime = String.Empty;

                        dateTime = (string)mySqlDataReader["ttrackerexpiry"];
                        tracker.dateExpired = Converter.subStandardDateTimeToDateTime(dateTime);
                        dateTime = String.Empty;

                        tracker.deviceImei = (string)mySqlDataReader["tunit"];
                        tracker.devicePassword = (string)mySqlDataReader["tunitpassword"];
                        tracker.deviceType = int.Parse((string)mySqlDataReader["ttype"]);
                        tracker.driverName = (string)mySqlDataReader["tdrivername"];
                        tracker.emails = (string)mySqlDataReader["temails"];
                        tracker.httpHost = int.Parse((string)mySqlDataReader["thttphost"]);
                        tracker.id = (int)mySqlDataReader["tid"];
                        tracker.idlingTime = int.Parse((string)mySqlDataReader["tidlingtime"]);
                        tracker.imageNumber = int.Parse((string)mySqlDataReader["timg"]);
                        tracker.inputs = (string)mySqlDataReader["tinputs"];
                        tracker.mileageInitial = int.Parse((string)mySqlDataReader["tmileageInit"]);
                        tracker.mileageLimit = int.Parse((string)mySqlDataReader["tmileagelimit"]);
                        tracker.mobileDataProvider = int.Parse((string)mySqlDataReader["tprovider"]);
                        tracker.note = (string)mySqlDataReader["tnote"];
                        tracker.ownerName = (string)mySqlDataReader["townername"];
                        tracker.simImei = (string)mySqlDataReader["tsimsr"];
                        tracker.simNumber = (string)mySqlDataReader["tsimno"];
                        tracker.users = (string)mySqlDataReader["tusers"];
                        tracker.speedLimit = int.Parse((string)mySqlDataReader["tspeedlimit"]);
                        tracker.vehicleModel = (string)mySqlDataReader["tvehiclemodel"];
                        tracker.vehicleRegistration = (string)mySqlDataReader["tvehiclereg"];


                        DataRow dataRow = dataTable.NewRow();
                        dataRow["id"] = tracker.id;
                        dataRow["vehicleRegistration"] = tracker.vehicleRegistration;
                        dataRow["vehicleModel"] = tracker.vehicleModel;
                        dataRow["ownerName"] = tracker.ownerName;
                        dataRow["driverName"] = tracker.driverName;

                        dataRow["simImei"] = tracker.simImei;
                        dataRow["simNumber"] = tracker.simNumber;
                        dataRow["mobileDataProvider"] = tracker.mobileDataProvider;

                        dataRow["deviceImei"] = tracker.deviceImei;
                        dataRow["devicePassword"] = tracker.devicePassword;
                        dataRow["deviceType"] = tracker.deviceType;

                        dataRow["emails"] = tracker.emails;
                        dataRow["users"] = tracker.users;

                        dataRow["mileageInitial"] = tracker.mileageInitial;
                        dataRow["mileageLimit"] = tracker.mileageLimit;
                        dataRow["speedLimit"] = tracker.speedLimit;

                        dataRow["idlingTime"] = tracker.idlingTime;
                        dataRow["inputs"] = tracker.inputs;
                        dataRow["imageNumber"] = tracker.imageNumber;
                        dataRow["note"] = tracker.note;

                        dataRow["collections"] = tracker.collections;
                        dataRow["companyDatabaseName"] = tracker.companyDatabaseName;
                        dataRow["databaseHost"] = tracker.databaseHost;
                        dataRow["dataDatabaseName"] = tracker.dataDatabaseName;
                        dataRow["httpHost"] = tracker.httpHost;
                        dataRow["dateCreated"] = tracker.dateCreated;
                        dataRow["dateExpired"] = tracker.dateExpired;

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

        public DataTable getAccounts(Account account) {
            DataTable dataTable = new DataTable();
            try {
                mysqlConnection.Open();
                string sql;
                if (account.accessLevel == 1 || account.accessLevel == 2) {
                    sql =
                        "SELECT  * " +
                        "FROM " + account.databaseName + ".usrs " +
                        "WHERE " + account.databaseName + ".usrs.upriv >= " + account.accessLevel.ToString() + ";";
                } else {
                    sql =
                       "SELECT  * " +
                       "FROM " + account.databaseName + ".usrs " +
                       "WHERE " + account.databaseName + ".usrs.upriv = " + account.accessLevel.ToString() + " " +
                       "AND " + account.databaseName + ".usrs.uname = '" + account.username + "';";
                }

                MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);

                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                if (!mySqlDataReader.HasRows) {
                    throw new QueryException(1, "Account's Collection is empty.");
                } else {
                    dataTable.Load(mySqlDataReader);
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

        public DataTable getTrackerHistoricalData(Account account, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, ReportType reportType, int limit, int offset, Tracker tracker) {
            DataTable dataTable = new DataTable();
            try {
                mysqlConnection.Open();

                double timestampFrom = Ats.Helper.Converter.dateTimeToUnixTimestamp(dateTimeDateFrom);
                double timestampTo = Ats.Helper.Converter.dateTimeToUnixTimestamp(dateTimeDateTo);

                string sql = "";
                if (timestampFrom > timestampTo) {
                    sql =
                       "SELECT " +

                       "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_id, " +
                       "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_time, " +
                       "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_lat, " +
                       "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_lng, " +
                       "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_ori, " +
                       "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_speed, " +
                       "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_mileage, " +
                       "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_data " +


                       "FROM trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + " " +
                       "WHERE trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_time >= " + timestampTo.ToString() + " " +
                       "AND trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_time <= " + timestampFrom.ToString() + " " +
                       "ORDER BY trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_time ASC LIMIT " + limit.ToString() + " OFFSET " + offset.ToString() + ";";
                } else {
                    sql =
                        "SELECT " +

                        "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_id, " +
                        "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_time, " +
                        "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_lat, " +
                        "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_lng, " +
                        "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_ori, " +
                        "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_speed, " +
                        "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_mileage, " +
                        "trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_data " +

                        "FROM trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + " " +
                        "WHERE trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_time <= " + timestampTo.ToString() + " " +
                        "AND trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_time >= " + timestampFrom.ToString() + " " +
                        "ORDER BY trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_time ASC LIMIT " + limit.ToString() + " OFFSET " + offset.ToString() + ";";
                }
                MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnection);

                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                dataTable.TableName = tracker.dataDatabaseName;
                dataTable.Columns.Add("No", typeof(int));
                dataTable.Columns.Add("DateTime", typeof(DateTime));
                dataTable.Columns.Add("Latitude", typeof(double));
                dataTable.Columns.Add("Longitude", typeof(double));
                dataTable.Columns.Add("Speed", typeof(int));
                dataTable.Columns.Add("Mileage", typeof(float));
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
                dataTable.Columns.Add("OS", typeof(bool));
                dataTable.Columns.Add("Battery", typeof(float));
                dataTable.Columns.Add("BatteryVolt", typeof(float));
                dataTable.Columns.Add("ExternalVolt", typeof(float));

                if (!mySqlDataReader.HasRows) {
                    //throw new QueryException(1, "Tracker data is empty.");
                    return dataTable;
                } else {
                    int index = offset;
                    while (mySqlDataReader.Read()) {
                        index++;

                        DataRow dataRow = dataTable.NewRow();
                        dataRow["No"] = index;
                        dataRow["DateTime"] = Converter.unixTimeStampToDateTime(float.Parse((string)mySqlDataReader["gm_time"]));

                        double latitude = double.Parse((string)mySqlDataReader["gm_lat"]);
                        double longitude = double.Parse((string)mySqlDataReader["gm_lng"]);

                        dataRow["Latitude"] = latitude;
                        dataRow["Longitude"] = longitude;
                        dataRow["Speed"] = int.Parse((string)mySqlDataReader["gm_speed"]);
                        dataRow["Mileage"] = Math.Round(float.Parse((string)mySqlDataReader["gm_mileage"]), 2);

                        if ((float)dataRow["Mileage"] < 0) {
                            dataRow["Mileage"] = ((float)dataRow["Mileage"]) * -1;
                        }

                        dataRow["Fuel"] = Math.Round(double.Parse((string)mySqlDataReader["gm_mileage"]) / Settings.Default.fuelLiterToKilometer, 2);
                        dataRow["Cost"] = Math.Round(((double)dataRow["Fuel"]) * Settings.Default.fuelLiterToCost, 2);
                        dataRow["Degrees"] = int.Parse((string)mySqlDataReader["gm_ori"]);
                        dataRow["Direction"] = Converter.degreesToCardinalDetailed(float.Parse((string)mySqlDataReader["gm_ori"]));

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
                        dataRow["OS"] = ((int)dataRow["Speed"] > tracker.speedLimit) ? true : false;
                        //Geofence
                        Location location = new Location(latitude, longitude);
                        MapTools mapTools = new MapTools();
                        foreach (Geofence geofence in account.geofenceCollection) {
                            if (mapTools.IsPointInPolygon(geofence, location)) {
                                if (String.IsNullOrEmpty(geofence.Name)) {
                                    dataRow["Geofence"] = "";

                                } else {
                                    dataRow["Geofence"] = geofence.Name;
                                }
                            }
                        };

                        double batteryStrength = (double)int.Parse(data[28], System.Globalization.NumberStyles.AllowHexSpecifier);
                        batteryStrength = ((batteryStrength - 2114f) * (100f / 492f));//*100.0;
                        batteryStrength = Math.Round(batteryStrength, 2);
                        if (batteryStrength > 100) {
                            batteryStrength = 100f;
                        } else if (batteryStrength < 0) {
                            batteryStrength = 0;
                        }

                        double batteryVoltage = (double)int.Parse(data[28], System.Globalization.NumberStyles.AllowHexSpecifier);
                        batteryVoltage = (batteryVoltage * 3 * 2) / 1024;
                        batteryVoltage = Math.Round(batteryVoltage, 2);
                        double externalVoltage = (double)int.Parse(data[29], System.Globalization.NumberStyles.AllowHexSpecifier);
                        externalVoltage = (externalVoltage * 3 * 16) / 1024;
                        externalVoltage = Math.Round(externalVoltage, 2);

                        dataRow["Battery"] = batteryStrength;
                        dataRow["BatteryVolt"] = batteryVoltage;
                        dataRow["ExternalVolt"] = externalVoltage;

                        dataTable.Rows.Add(dataRow);
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

        public int getTrackerHistoricalDataCount(Account account, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, ReportType reportType, Tracker tracker) {
            DataTable dataTable = new DataTable();
            try {
                mysqlConnection.Open();

                double timestampFrom = Ats.Helper.Converter.dateTimeToUnixTimestamp(dateTimeDateFrom);
                double timestampTo = Ats.Helper.Converter.dateTimeToUnixTimestamp(dateTimeDateTo);

                string sql = "";
                if (timestampFrom > timestampTo) {
                    sql =
                       "SELECT  COUNT(*) " +
                       "FROM trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + " " +
                       "WHERE trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_time >= " + timestampTo.ToString() + " " +
                       "AND trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_time <= " + timestampFrom.ToString() + " " +
                       "ORDER BY trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_id ASC;";


                } else {
                    sql =
                          "SELECT  COUNT(*) " +
                          "FROM trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + " " +
                          "WHERE trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_time <= " + timestampTo.ToString() + " " +
                          "AND trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_time >= " + timestampFrom.ToString() + " " +
                          "ORDER BY trk_" + tracker.dataDatabaseName + ".gps_" + tracker.dataDatabaseName + ".gm_id ASC;";

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

        public DataTable getTrackerGeofence(Account account, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, ReportType reportType, int limit, int offset, Tracker tracker) {
            DataTable dataTableHistoricalData = this.getTrackerHistoricalData(account, dateTimeDateFrom, dateTimeDateTo, reportType, limit, offset, tracker);
            DataTable dataTableGeofenceData = new DataTable();
            dataTableGeofenceData.Columns.Add("No", typeof(int));
            dataTableGeofenceData.Columns.Add("Status", typeof(bool));
            dataTableGeofenceData.Columns.Add("DateTimeFrom", typeof(DateTime));
            dataTableGeofenceData.Columns.Add("DateTimeTo", typeof(DateTime));
            dataTableGeofenceData.Columns.Add("Time", typeof(TimeSpan));
            dataTableGeofenceData.Columns.Add("SpeedMax", typeof(double));
            dataTableGeofenceData.Columns.Add("SpeedAve", typeof(double));
            dataTableGeofenceData.Columns.Add("Distance", typeof(double));
            dataTableGeofenceData.Columns.Add("Fuel", typeof(double));
            dataTableGeofenceData.Columns.Add("Cost", typeof(double));
            dataTableGeofenceData.Columns.Add("Geofence", typeof(string));

            try {

                string geofenceStatusNow = "";
                string geofenceStatusBefore = "";

                //EventCode eventCode = EventCode.TRACK_BY_TIME_INTERVAL;
                bool acc = false;

                int index = 0;

                double speed = 0;
                double speedDivisor = 0;
                double speedAccumulator = 0;
                double speedMax = 0;
                double speedAverage = 0;


                double distanceBefore = 0;
                double distance = 0;

                string geofence;
                DateTime dateTimeGeofenceFrom = new DateTime();
                DateTime dateTimeGeofenceTo = new DateTime();

                TimeSpan timeSpan;
                //string accumulatedIdleTime;

                foreach (DataRow dataRowNow in dataTableHistoricalData.Rows) {
                    if ((int)dataRowNow["No"] == 1) {
                        dateTimeGeofenceFrom = (DateTime)dataRowNow["DateTime"];
                        distanceBefore = (double)(float)dataRowNow["Mileage"];
                    }

                    //if (!Convert.IsDBNull(dataRowNow["EventCode"])) {
                    //    eventCode = (EventCode)Enum.Parse(typeof(EventCode), (string)dataRowNow["EventCode"], true);
                    //} else {
                    //    geofenceStatusNow = "";
                    //}


                    acc = (bool)dataRowNow["ACC"];

                    speed = (int)dataRowNow["Speed"];
                    speedDivisor++;
                    speedAccumulator += speed;
                    if (speed > speedMax) {
                        speedMax = speed;
                    }

                    object obj = dataRowNow["Geofence"].GetType();

                    if (!Convert.IsDBNull(dataRowNow["Geofence"])) {
                        geofenceStatusNow = (string)dataRowNow["Geofence"];
                    } else {
                        geofenceStatusNow = "";
                    }


                    if (geofenceStatusBefore != geofenceStatusNow) {
                        index++;
                        geofenceStatusBefore = geofenceStatusNow;
                        dateTimeGeofenceTo = (DateTime)dataRowNow["DateTime"];
                        geofence = geofenceStatusNow;//(dataRowNow["Geofence"] == System.DBNull.Value) ? "" : (string)dataRowNow["Geofence"];
                        timeSpan = dateTimeGeofenceTo - dateTimeGeofenceFrom;
                        distance = (double)(float)dataRowNow["Mileage"] - distanceBefore;
                        distance = Math.Round(distance, 2);
                        //accumulatedIdleTime = timeSpan.ToString(@"dd\ hh\:mm\:ss");

                        speedAverage = speedAccumulator / speedDivisor;
                        speedAverage = Math.Round(speedAverage, 2);
                        speedAccumulator = 0;
                        speedDivisor = 0;


                        DataRow dataRowGeofenceData = dataTableGeofenceData.NewRow();
                        dataRowGeofenceData["No"] = index;

                        dataRowGeofenceData["DateTimeFrom"] = dateTimeGeofenceFrom;
                        dataRowGeofenceData["DateTimeTo"] = dateTimeGeofenceTo;
                        dataRowGeofenceData["Time"] = timeSpan;
                        dataRowGeofenceData["Distance"] = distance;


                        dataRowGeofenceData["Fuel"] = Math.Round(((double)dataRowGeofenceData["Distance"]) / Settings.Default.fuelLiterToKilometer, 2);
                        dataRowGeofenceData["Cost"] = Math.Round(((double)dataRowGeofenceData["Fuel"]) / Settings.Default.fuelLiterToCost, 2);


                        dataRowGeofenceData["Geofence"] = geofence;
                        dataRowGeofenceData["SpeedMax"] = speedMax;
                        dataRowGeofenceData["SpeedAve"] = speedAverage;
                        dataRowGeofenceData["Status"] = geofence == "" ? false : true;

                        speedMax = 0;
                        distanceBefore = distance + distanceBefore;
                        dateTimeGeofenceFrom = dateTimeGeofenceTo;
                        dataTableGeofenceData.Rows.Add(dataRowGeofenceData);
                    }
                }

                return dataTableGeofenceData;

            } catch (QueryException queryException) {
                throw queryException;
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            }
        }

        public DataTable getTrackerIdleData(Account account, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, ReportType reportType, int limit, int offset, Tracker tracker) {
            DataTable dataTableHistoricalData = this.getTrackerHistoricalData(account, dateTimeDateFrom, dateTimeDateTo, reportType, limit, offset, tracker);
            DataTable dataTableIdleData = new DataTable();
            dataTableIdleData.Columns.Add("No", typeof(int));
            dataTableIdleData.Columns.Add("Status", typeof(bool));
            dataTableIdleData.Columns.Add("DateTimeFrom", typeof(DateTime));
            dataTableIdleData.Columns.Add("DateTimeTo", typeof(DateTime));
            dataTableIdleData.Columns.Add("Time", typeof(TimeSpan));
            dataTableIdleData.Columns.Add("SpeedMax", typeof(double));
            dataTableIdleData.Columns.Add("SpeedAve", typeof(double));
            dataTableIdleData.Columns.Add("Distance", typeof(double));
            dataTableIdleData.Columns.Add("Fuel", typeof(double));
            dataTableIdleData.Columns.Add("Cost", typeof(double));
            dataTableIdleData.Columns.Add("Geofence", typeof(string));

            try {

                bool idleStatusNow = false;
                bool idleStatusBefore = false;

                EventCode eventCode = EventCode.TRACK_BY_TIME_INTERVAL;
                bool acc = false;

                int index = 0;

                double speed = 0;
                double speedDivisor = 0;
                double speedAccumulator = 0;
                double speedMax = 0;
                double speedAverage = 0;


                double distanceBefore = 0;
                double distance = 0;

                string geofence;
                DateTime dateTimeIdleFrom = new DateTime();
                DateTime dateTimeIdleTo = new DateTime();

                TimeSpan timeSpan;

                foreach (DataRow dataRowNow in dataTableHistoricalData.Rows) {
                    if ((int)dataRowNow["No"] == 1) {
                        dateTimeIdleFrom = (DateTime)dataRowNow["DateTime"];
                        distanceBefore = (double)(float)dataRowNow["Mileage"];
                    }

                    eventCode = (dataRowNow["EventCode"] == System.DBNull.Value) ?
                       EventCode.TRACK_BY_TIME_INTERVAL : (EventCode)Enum.Parse(typeof(EventCode), (string)dataRowNow["EventCode"], true);

                    acc = (bool)dataRowNow["ACC"];

                    speed = (int)dataRowNow["Speed"];
                    speedDivisor++;
                    speedAccumulator += speed;
                    if (speed > speedMax) {
                        speedMax = speed;
                    }


                    if (speed == 0 && acc == true) {
                        idleStatusNow = true;
                    }

                    if (speed > 0 && acc == false) {
                        idleStatusNow = false;
                    }


                    if (idleStatusBefore != idleStatusNow) {
                        index++;
                        idleStatusBefore = idleStatusNow;
                        dateTimeIdleTo = (DateTime)dataRowNow["DateTime"];
                        geofence = (dataRowNow["Geofence"] == System.DBNull.Value) ? "" : (string)dataRowNow["Geofence"];
                        timeSpan = dateTimeIdleTo - dateTimeIdleFrom;
                        distance = (double)(float)dataRowNow["Mileage"] - distanceBefore;
                        distance = Math.Round(distance, 2);

                        speedAverage = speedAccumulator / speedDivisor;
                        speedAverage = Math.Round(speedAverage, 2);
                        speedAccumulator = 0;
                        speedDivisor = 0;


                        DataRow dataRowIdleData = dataTableIdleData.NewRow();
                        dataRowIdleData["No"] = index;
                        dataRowIdleData["Status"] = idleStatusNow;
                        dataRowIdleData["DateTimeFrom"] = dateTimeIdleFrom;
                        dataRowIdleData["DateTimeTo"] = dateTimeIdleTo;
                        dataRowIdleData["Time"] = timeSpan;
                        dataRowIdleData["Distance"] = distance;
                        dataRowIdleData["Geofence"] = geofence;

                        dataRowIdleData["Fuel"] = Math.Round(((double)dataRowIdleData["Distance"]) / Settings.Default.fuelLiterToKilometer, 2);
                        dataRowIdleData["Cost"] = Math.Round(((double)dataRowIdleData["Fuel"]) / Settings.Default.fuelLiterToCost, 2);

                        dataRowIdleData["SpeedMax"] = speedMax;
                        dataRowIdleData["SpeedAve"] = speedAverage;

                        speedMax = 0;
                        distanceBefore = distance + distanceBefore;
                        dateTimeIdleFrom = dateTimeIdleTo;
                        dataTableIdleData.Rows.Add(dataRowIdleData);
                    }
                }


            } catch (QueryException queryException) {
                throw queryException;
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {

            }
            return dataTableIdleData;
        }

        public DataTable getTrackerRunningData(Account account, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, ReportType reportType, int limit, int offset, Tracker tracker) {
            DataTable dataTableHistoricalData = this.getTrackerHistoricalData(account, dateTimeDateFrom, dateTimeDateTo, reportType, limit, offset, tracker);
            DataTable dataTableIdleData = new DataTable();
            dataTableIdleData.Columns.Add("No", typeof(int));
            dataTableIdleData.Columns.Add("Status", typeof(bool));
            dataTableIdleData.Columns.Add("DateTimeFrom", typeof(DateTime));
            dataTableIdleData.Columns.Add("DateTimeTo", typeof(DateTime));
            dataTableIdleData.Columns.Add("Time", typeof(TimeSpan));
            dataTableIdleData.Columns.Add("SpeedMax", typeof(double));
            dataTableIdleData.Columns.Add("SpeedAve", typeof(double));
            dataTableIdleData.Columns.Add("Distance", typeof(double));
            dataTableIdleData.Columns.Add("Fuel", typeof(double));
            dataTableIdleData.Columns.Add("Cost", typeof(double));
            dataTableIdleData.Columns.Add("Geofence", typeof(string));

            try {

                bool runningStatusNow = false;
                bool runningStatusBefore = false;
                EventCode eventCode = EventCode.TRACK_BY_TIME_INTERVAL;
                bool acc = false;

                int index = 0;

                double speed = 0;
                double speedDivisor = 0;
                double speedAccumulator = 0;
                double speedMax = 0;
                double speedAverage = 0;


                double distanceBefore = 0;
                double distance = 0;

                string geofence;
                DateTime dateTimeRunningFrom = new DateTime();
                DateTime dateTimeRunningTo = new DateTime();

                TimeSpan timeSpan;

                foreach (DataRow dataRowNow in dataTableHistoricalData.Rows) {
                    if ((int)dataRowNow["No"] == 1) {
                        dateTimeRunningFrom = (DateTime)dataRowNow["DateTime"];
                        distanceBefore = (double)(float)dataRowNow["Mileage"];
                    }

                    eventCode = (dataRowNow["EventCode"] == System.DBNull.Value) ?
                       EventCode.TRACK_BY_TIME_INTERVAL : (EventCode)Enum.Parse(typeof(EventCode), (string)dataRowNow["EventCode"], true);

                    acc = (bool)dataRowNow["ACC"];

                    speed = (int)dataRowNow["Speed"];
                    speedDivisor++;
                    speedAccumulator += speed;
                    if (speed > speedMax) {
                        speedMax = speed;
                    }


                    if (acc == true && speed > 0) {
                        runningStatusNow = true;
                    }

                    if (acc = true && speed <= 0) {
                        runningStatusNow = false;
                    }

                    if (acc = false && speed <= 0) {
                        runningStatusNow = false;
                    }
                    if (runningStatusBefore != runningStatusNow) {
                        runningStatusBefore = runningStatusNow;
                        index++;
                        dateTimeRunningTo = (DateTime)dataRowNow["DateTime"];
                        geofence = (dataRowNow["Geofence"] == System.DBNull.Value) ? "" : (string)dataRowNow["Geofence"];
                        timeSpan = dateTimeRunningTo - dateTimeRunningFrom;
                        distance = (double)(float)dataRowNow["Mileage"] - distanceBefore;
                        distance = Math.Round(distance, 2);


                        speedAverage = speedAccumulator / speedDivisor;
                        speedAverage = Math.Round(speedAverage, 2);
                        speedAccumulator = 0;
                        speedDivisor = 0;


                        DataRow dataRowIdleData = dataTableIdleData.NewRow();
                        dataRowIdleData["No"] = index;
                        dataRowIdleData["Status"] = runningStatusNow;
                        dataRowIdleData["DateTimeFrom"] = dateTimeRunningFrom;
                        dataRowIdleData["DateTimeTo"] = dateTimeRunningTo;
                        dataRowIdleData["Time"] = timeSpan;
                        dataRowIdleData["Distance"] = distance;

                        dataRowIdleData["Fuel"] = Math.Round(((double)dataRowIdleData["Distance"]) / Settings.Default.fuelLiterToKilometer, 2);
                        dataRowIdleData["Cost"] = Math.Round(((double)dataRowIdleData["Fuel"]) / Settings.Default.fuelLiterToCost, 2);

                        dataRowIdleData["Geofence"] = geofence;
                        dataRowIdleData["SpeedMax"] = speedMax;
                        dataRowIdleData["SpeedAve"] = speedAverage;

                        speedMax = 0;
                        distanceBefore = distance + distanceBefore;
                        dateTimeRunningFrom = dateTimeRunningTo;
                        dataTableIdleData.Rows.Add(dataRowIdleData);
                    }
                }
            } catch (QueryException queryException) {
                throw queryException;
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {

            }
            return dataTableIdleData;
        }

        public DataTable getTrackerAccData(Account account, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, ReportType reportType, int limit, int offset, Tracker tracker) {
            DataTable dataTableHistoricalData = this.getTrackerHistoricalData(account, dateTimeDateFrom, dateTimeDateTo, reportType, limit, offset, tracker);
            DataTable dataTableIdleData = new DataTable();
            dataTableIdleData.Columns.Add("No", typeof(int));
            dataTableIdleData.Columns.Add("Status", typeof(bool));
            dataTableIdleData.Columns.Add("DateTimeFrom", typeof(DateTime));
            dataTableIdleData.Columns.Add("DateTimeTo", typeof(DateTime));
            dataTableIdleData.Columns.Add("Time", typeof(TimeSpan));
            dataTableIdleData.Columns.Add("SpeedMax", typeof(double));
            dataTableIdleData.Columns.Add("SpeedAve", typeof(double));
            dataTableIdleData.Columns.Add("Distance", typeof(double));
            dataTableIdleData.Columns.Add("Fuel", typeof(double));
            dataTableIdleData.Columns.Add("Cost", typeof(double));
            dataTableIdleData.Columns.Add("Geofence", typeof(string));

            try {

                bool accStatusNow = false;
                bool accStatusBefore = false;


                int index = 0;

                double speed = 0;
                double speedDivisor = 0;
                double speedAccumulator = 0;
                double speedMax = 0;
                double speedAverage = 0;


                double distanceBefore = 0;
                double distance = 0;

                string geofence;
                DateTime dateTimeRunningFrom = new DateTime();
                DateTime dateTimeRunningTo = new DateTime();

                TimeSpan timeSpan;


                foreach (DataRow dataRowNow in dataTableHistoricalData.Rows) {
                    if ((int)dataRowNow["No"] == 1) {
                        dateTimeRunningFrom = (DateTime)dataRowNow["DateTime"];
                        distanceBefore = (double)(float)dataRowNow["Mileage"];
                    }

                    accStatusNow = (bool)dataRowNow["ACC"];

                    speed = (int)dataRowNow["Speed"];
                    speedDivisor++;
                    speedAccumulator += speed;
                    if (speed > speedMax) {
                        speedMax = speed;
                    }



                    if (accStatusBefore != accStatusNow) {
                        accStatusBefore = accStatusNow;
                        index++;
                        dateTimeRunningTo = (DateTime)dataRowNow["DateTime"];
                        geofence = (dataRowNow["Geofence"] == System.DBNull.Value) ? "" : (string)dataRowNow["Geofence"];
                        timeSpan = dateTimeRunningTo - dateTimeRunningFrom;
                        distance = (double)(float)dataRowNow["Mileage"] - distanceBefore;
                        distance = Math.Round(distance, 2);


                        speedAverage = speedAccumulator / speedDivisor;
                        speedAverage = Math.Round(speedAverage, 2);
                        speedAccumulator = 0;
                        speedDivisor = 0;


                        DataRow dataRowIdleData = dataTableIdleData.NewRow();
                        dataRowIdleData["No"] = index;
                        dataRowIdleData["Status"] = !accStatusNow;
                        dataRowIdleData["DateTimeFrom"] = dateTimeRunningFrom;
                        dataRowIdleData["DateTimeTo"] = dateTimeRunningTo;
                        dataRowIdleData["Time"] = timeSpan;
                        dataRowIdleData["Distance"] = distance;

                        dataRowIdleData["Fuel"] = Math.Round(((double)dataRowIdleData["Distance"]) / Settings.Default.fuelLiterToKilometer, 2);
                        dataRowIdleData["Cost"] = Math.Round(((double)dataRowIdleData["Fuel"]) / Settings.Default.fuelLiterToCost, 2);

                        dataRowIdleData["Geofence"] = geofence;
                        dataRowIdleData["SpeedMax"] = speedMax;
                        dataRowIdleData["SpeedAve"] = speedAverage;

                        speedMax = 0;
                        distanceBefore = distance + distanceBefore;
                        dateTimeRunningFrom = dateTimeRunningTo;
                        dataTableIdleData.Rows.Add(dataRowIdleData);
                    }
                }
            } catch (QueryException queryException) {
                throw queryException;
            } catch (Exception exception) {
                throw new QueryException(1, exception.Message);
            } finally {

            }
            return dataTableIdleData;
        }

        public DataTable getTrackerOverSpeedData(Account account, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, ReportType reportType, int limit, int offset, Tracker tracker) {
            DataTable dataTableHistoricalData = this.getTrackerHistoricalData(account, dateTimeDateFrom, dateTimeDateTo, reportType, limit, offset, tracker);
            DataTable dataTableOverSpeedData = new DataTable();
            dataTableOverSpeedData.Columns.Add("No", typeof(int));
            dataTableOverSpeedData.Columns.Add("Status", typeof(bool));
            dataTableOverSpeedData.Columns.Add("DateTime", typeof(DateTime));
            dataTableOverSpeedData.Columns.Add("Latitude", typeof(Double));
            dataTableOverSpeedData.Columns.Add("Longitude", typeof(Double));
            dataTableOverSpeedData.Columns.Add("Speed", typeof(int));
            dataTableOverSpeedData.Columns.Add("Mileage", typeof(float));
            dataTableOverSpeedData.Columns.Add("Geofence", typeof(string));

            try {

                bool overSpeedStatusNow = false;
                int index = 0;
                string geofence;


                foreach (DataRow dataRowNow in dataTableHistoricalData.Rows) {

                    overSpeedStatusNow = (bool)dataRowNow["OS"];

                    if (overSpeedStatusNow) {

                        index++;
                        geofence = (dataRowNow["Geofence"] == System.DBNull.Value) ? "" : (string)dataRowNow["Geofence"];


                        DataRow dataRowOverspeedData = dataTableOverSpeedData.NewRow();
                        dataRowOverspeedData["No"] = index;
                        dataRowOverspeedData["Status"] = overSpeedStatusNow;
                        dataRowOverspeedData["DateTime"] = (DateTime)dataRowNow["DateTime"];
                        dataRowOverspeedData["Latitude"] = (double)dataRowNow["Latitude"];
                        dataRowOverspeedData["Longitude"] = (double)dataRowNow["Longitude"];
                        dataRowOverspeedData["Speed"] = (int)dataRowNow["Speed"];
                        dataRowOverspeedData["Mileage"] = (float)dataRowNow["Mileage"];
                        dataRowOverspeedData["Geofence"] = geofence;
                        ;
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

        public DataTable getTrackersGeofence(Account account, DateTime dateTimeDateFrom, DateTime dateTimeDateTo, ReportType reportType, int limit, int offset, List<Tracker> trackerList) {

            DataTable dataTable = new DataTable();
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

            try {
                int index = 0;
                foreach (Tracker tracker in trackerList) {
                    index++;

                    DataTable dataTableGeofence = (this.getTrackerGeofence(account, dateTimeDateFrom, dateTimeDateTo, reportType, limit, offset, tracker));

                    if (dataTableGeofence.Rows.Count <= 0) {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["No"] = index;
                        dataRow["VehicleRegistration"] = tracker.vehicleRegistration;
                        dataRow["DriverName"] = tracker.driverName;
                        dataRow["DeviceImei"] = tracker.deviceImei;

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
                        dataRow["VehicleRegistration"] = tracker.vehicleRegistration;
                        dataRow["DriverName"] = tracker.driverName;
                        dataRow["DeviceImei"] = tracker.deviceImei;

                        dataRow["DateTimeFrom"] = dateTimeDateFrom;
                        dataRow["DateTimeTo"] = dateTimeDateTo;

                        double speedAve = 0;
                        if (dataTableGeofence.Rows.Count > 0)
                            speedAve = Converter.dataTableColumnSumValue(dataTableGeofence, dataTableGeofence.Columns["SpeedAve"]) / dataTableGeofence.Rows.Count;
                        dataRow["SpeedAve"] = Math.Round(speedAve, 2);

                        double speedMax = Converter.dataTableColumnMaxValue(dataTableGeofence, dataTableGeofence.Columns["SpeedMax"]);
                        dataRow["SpeedMax"] = Math.Round(speedMax, 2);

                        dataRow["Distance"] = Math.Round(Converter.dataTableColumnSumValue(dataTableGeofence, dataTableGeofence.Columns["Distance"]), 2);
                        dataRow["Fuel"] = Math.Round(Converter.dataTableColumnSumValue(dataTableGeofence, dataTableGeofence.Columns["Fuel"]), 2);
                        dataRow["Cost"] = Math.Round(Converter.dataTableColumnSumValue(dataTableGeofence, dataTableGeofence.Columns["Cost"]), 2);

                        dataRow["Geofence"] = Converter.dataTableColumnAppend(dataTableGeofence, dataTableGeofence.Columns["Geofence"], ":");
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

    }
}
