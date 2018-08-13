using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matchmaker.Net.Client;
using Matchmaker.Net.Configuration;
using Matchmaker.Net.Debug;

namespace Matchmaker.Net.Server
{
    public static class AntispamProtection
    {
        private static Dictionary<string, int> malformUsers = new Dictionary<string, int>();

        public static void listSpamUser()
        {
            Logging.dbgMessage("<SpamList>");
            foreach (var x in malformUsers)
                Logging.dbgMessage(String.Format(":: [Banned] ip {0} with malcount {2}", x.Key, x.Value));
            Logging.dbgMessage("</SpamList>");
        }

        public static void clearSpamUser()
        {
            malformUsers.Clear();
        }

        public static bool checkUser(string ip)
        {
            if (malformUsers.ContainsKey(ip))
            {
                if(malformUsers[ip] > Configuration.SpamProtection.FAILED_ATTEMPT_COUNT)
                {
                    Debug.Logging.errlog("User " + ip + "denied acceess due to repeated malformed data. ("+ malformUsers[ip]+ " failures)", Enums.ErrorSeverity.ERROR_INFO);
                    return false;
                }

                return true;
            }

            return true;
        }

        public static void markForMaloformedData(string ip)
        {
            if (malformUsers.ContainsKey(ip))
            {
                malformUsers[ip]++;
            }
            else
            {
                malformUsers.Add(ip, 1);
            }
        }

        public static void clearBanUser()
        {
            malformUsers.Clear();
        }
    }
}
