using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace Ats.Helper {
    class NetworkTools {
        public static bool ping(string sString) {
            PingReply pingReply;
            Ping ping = new Ping();

            try {
                pingReply = ping.Send(sString);
            } catch (Exception) {
                return false;
            }

            return pingReply.Status == IPStatus.Success;

        }
    }
}
