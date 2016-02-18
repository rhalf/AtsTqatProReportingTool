using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace Ats.Helper {
    class StringValidator {
   

        public static bool isIpAddress(string sIpAddress) {
            IPAddress unused;
            return IPAddress.TryParse(sIpAddress, out unused) &&
              (
                  unused.AddressFamily != AddressFamily.InterNetwork
                  ||
                  sIpAddress.Count(c => c == '.') == 3
              );

        }

    }


}
