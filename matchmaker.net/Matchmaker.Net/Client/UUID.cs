using System;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Matchmaker.Net.Client
{
    public class UUID
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);

        private string _UNIQUE_IDENTITY;
        private string _HWID;

        public UUID()
        {
            try
            {
                var ManagmentService = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
                ManagementObjectCollection mbsList = ManagmentService.Get();

                string CPUid = "";
                foreach (ManagementObject mo in mbsList)
                {
                    CPUid = mo["ProcessorId"].ToString();
                    break;
                }

                _HWID = CPUid;

                MD5 hashGenerator = MD5.Create();

                var hwidBytes = Encoding.ASCII.GetBytes(CPUid);
                var hashBytes = hashGenerator.ComputeHash(hwidBytes);

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)

                {

                    sb.Append(hashBytes[i].ToString("X2"));

                }
                _UNIQUE_IDENTITY = sb.ToString();
            }
            catch (Exception e)
            {
                Debug.Logging.errlog("Unable to find HWID. Device not supported.", Enums.ErrorSeverity.ERROR_CRITICAL_FAILURE);
                MessageBox((IntPtr)0, "Unable to find HWID. Device not supported.\n\n" + e.Message + "\n" + e.StackTrace, "Unable to start matchmaking server", 0);
                Environment.Exit(1001);
            }
        }

        public string GetHWID()
        {
            return _HWID;
        }

        public string GetUUID()
        {
            return _UNIQUE_IDENTITY;
        }

    }
}
