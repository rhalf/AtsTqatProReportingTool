using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Data;

namespace Ats.Helper {

    public enum LogFileType {
        TXT,
        CSV
    }

    public enum LogType {
        NONE,
        EVENT,
        EXCEPTION
    }

    class Log {
        string _path, _parentDirectory, _fileName, _identifier;
        LogFileType _logFileType;
        LogType _logType;
        public Log(LogFileType logFileType, LogType logType) {
          
            this._logFileType = logFileType;
            this._logType = logType;
        }
        public string path {
            get {
                return this._path;
            }
            set {
                this._path = value;
            }
        }
        public string parentDirectory {
            get {
                return this._parentDirectory;
            }
            set {
                this._parentDirectory = value;
            }
        }
        public string identifier {
            get {
                return this._identifier;
            }
            set {
                this._identifier = value;
            }
        }
        public void write(string logMessage) {

            this._path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\TqatProReportingTool\\logs";

            if (!Directory.Exists(this._path)) {
                Directory.CreateDirectory(this._path);
            }

            string logType = Enum.GetName(typeof(LogType), this._logType);
            string logFileType = Enum.GetName(typeof(LogFileType), this._logFileType);


            this._fileName = this._path + "\\" + logType + "_" + DateTime.Now.ToString("yyyyMMdd") + "." + logFileType;


            if (!File.Exists(this._fileName)) {
                try {
                    using (StreamWriter streamWriter = new StreamWriter(this._fileName, false)) {
                        streamWriter.WriteLine(logMessage);
                    }
                } catch {
                    //Do nothing
                }

            } else {

                try {
                    using (StreamWriter streamWriter = new StreamWriter(this._fileName, true)) {
                        streamWriter.WriteLine(logMessage);
                    }
                } catch {
                    //Do nothing
                }
            }

        }
    }



}
