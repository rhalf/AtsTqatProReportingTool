using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Ats.Helper {
    class Cryptography {

        public static string md5(string sInput) {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] aInputBytes = System.Text.Encoding.ASCII.GetBytes(sInput);
            byte[] aHash = md5.ComputeHash(aInputBytes);

            StringBuilder sbOutput = new StringBuilder();
            for (int i = 0; i < aHash.Length; i++) {
                sbOutput.Append(aHash[i].ToString("X2"));
            }

            return sbOutput.ToString();
        }
    }
}
