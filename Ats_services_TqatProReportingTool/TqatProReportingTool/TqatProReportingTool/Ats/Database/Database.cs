using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ats.Helper;
using MySql.Data.MySqlClient;

namespace Ats.Database {
    public class Database {
        private string databaseName = null;
        private string host = null;
        private string username = null;
        private string password = null;
        private int port = 0;
        private int commandTimeOut = 0;

        public Database (string sHost, string sUsername, string sPassword, int iPort, string sDatabaseName, int commandTimeout) {
            if (String.IsNullOrEmpty(sHost)) {
                throw new DatabaseException(1, "Host is Null or Empty!");
            }
            if (String.IsNullOrEmpty(sUsername)) {
                throw new DatabaseException(1, "Username is Null or Empty!");
            }
            if (String.IsNullOrEmpty(sPassword)) {
                throw new DatabaseException(1, "Password is Null or Empty!");
            }
            if (String.IsNullOrEmpty(sDatabaseName)) {
                throw new DatabaseException(1, "DatabaseName is Null or Empty!");
            }
            if (iPort < 1 || iPort > 65536) {
                throw new DatabaseException(1, "Port should be from 1-65536!");
            }

            this.host = sHost;
            this.username = sUsername;
            this.password = sPassword;
            this.port = iPort;
            this.databaseName = sDatabaseName;
            this.commandTimeOut = commandTimeout;

        }

        public Database (string sHost, string sUsername, string sPassword, int iPort, string sDatabaseName) {
            if (String.IsNullOrEmpty(sHost)) {
                throw new DatabaseException(1, "Host is Null or Empty!");
            }
            if (String.IsNullOrEmpty(sUsername)) {
                throw new DatabaseException(1, "Username is Null or Empty!");
            }
            if (String.IsNullOrEmpty(sPassword)) {
                throw new DatabaseException(1, "Password is Null or Empty!");
            }
            //if (String.IsNullOrEmpty(sDatabaseName)) {
            //    throw new DatabaseException(1, "DatabaseName is Null or Empty!");
            //}
            if (iPort < 1 || iPort > 65536) {
                throw new DatabaseException(1, "Port should be from 1-65536!");
            }

            this.host = sHost;
            this.username = sUsername;
            this.password = sPassword;
            this.port = iPort;
            this.databaseName = sDatabaseName;
            this.commandTimeOut = 30;

        }
        public Database (string sHost, string sUsername, string sPassword, int commandTimeout) {
            if (String.IsNullOrEmpty(sHost)) {
                throw new DatabaseException(1, "Host is Null or Empty!");
            }
            if (String.IsNullOrEmpty(sUsername)) {
                throw new DatabaseException(1, "Username is Null or Empty!");
            }
            if (String.IsNullOrEmpty(sPassword)) {
                throw new DatabaseException(1, "Password is Null or Empty!");
            }

            this.host = sHost;
            this.username = sUsername;
            this.password = sPassword;
            this.port = 3306;
            this.databaseName = null;
            this.commandTimeOut = commandTimeout;
        }
        public Database (string sHost, string sUsername, string sPassword) {
            if (String.IsNullOrEmpty(sHost)) {
                throw new DatabaseException(1, "Host is Null or Empty!");
            }
            if (String.IsNullOrEmpty(sUsername)) {
                throw new DatabaseException(1, "Username is Null or Empty!");
            }
            if (String.IsNullOrEmpty(sPassword)) {
                throw new DatabaseException(1, "Password is Null or Empty!");
            }

            this.host = sHost;
            this.username = sUsername;
            this.password = sPassword;
            this.port = 3306;
            this.databaseName = null;
            this.commandTimeOut = 30;
        }

        public string getConnectionString () {
            string sConnectionString;
            if (databaseName != null) {
                sConnectionString = "SERVER=" + this.host + ";" + "DATABASE=" + this.databaseName + ";" + "UID=" + this.username + ";" + "PASSWORD=" + this.password + ";";
            } else {
                sConnectionString = "SERVER=" + this.host + ";" + "DATABASE=;" + "UID=" + this.username + ";" + "PASSWORD=" + this.password + "; default command timeout=" + this.commandTimeOut;
            }

            return sConnectionString;
        }

        public void checkConnection () {
            if (String.IsNullOrEmpty(host)) {
                throw new DatabaseException(1, "Host is Null or Empty!");
            }
            if (String.IsNullOrEmpty(username)) {
                throw new DatabaseException(1, "Username is Null or Empty!");
            }
            if (String.IsNullOrEmpty(password)) {
                throw new DatabaseException(1, "Password is Null or Empty!");
            }
            if (port < 1 || port > 65536) {
                throw new DatabaseException(1, "Port should be from 1-65536!");
            }

            MySqlConnection mysqlConnection = new MySqlConnection(this.getConnectionString());

            try {
                mysqlConnection.Open();
            } catch (MySqlException mySqlException) {
                throw new DatabaseException(1, mySqlException.Message);
            } catch (Exception exception) {
                throw new DatabaseException(1, exception.Message);
            } finally {
                mysqlConnection.Close();
            }
        }

    }
}
