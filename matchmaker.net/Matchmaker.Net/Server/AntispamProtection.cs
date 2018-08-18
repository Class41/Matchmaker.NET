using Matchmaker.Net.Debug;
using System;
using System.Collections.Generic;

namespace Matchmaker.Net.Server
{
    public static class AntispamProtection
    {
        private static Dictionary<string, int> _malformUsers = new Dictionary<string, int>();

        public static void ListSpamUser()
        {
            Logging.dbgMessage("<SpamList>");
            foreach (var x in _malformUsers)
                Logging.dbgMessage(String.Format(":: [Banned] ip {0} with malcount {1}", x.Key, x.Value));
            Logging.dbgMessage("</SpamList>");
        }

        public static void ClearSpamUser()
        {
            _malformUsers.Clear();
        }

        public static bool CheckUser(string ip)
        {
            if (_malformUsers.ContainsKey(ip))
            {
                if (_malformUsers[ip] >= Configuration.SpamProtection.FAILED_ATTEMPT_COUNT_MAX)
                {
                    Debug.Logging.errlog(String.Format("User {0} denied acceess due to repeated malformed data. ({1} failures)", ip, _malformUsers[ip]), Enums.ErrorSeverity.ERROR_INFO);
                    return false;
                }

                return true;
            }

            return true;
        }

        public static void MarkForMaloformedData(string ip)
        {
            if (_malformUsers.ContainsKey(ip))
            {
                _malformUsers[ip]++;
            }
            else
            {
                _malformUsers.Add(ip, 1);
            }
        }

        public static void ClearBanUser()
        {
            _malformUsers.Clear();
        }
    }
}
