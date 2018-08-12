using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Security.Cryptography;

namespace Matchmaker.Net.Client
{
    public class UUID
    {
        private string UNIQUE_IDENTITY;
        private string HWID;

        public UUID()
        {
            var ManagmentService = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            ManagementObjectCollection mbsList = ManagmentService.Get();

            string CPUid = "";
            foreach (ManagementObject mo in mbsList)
            {
                CPUid = mo["ProcessorId"].ToString();
                break;
            }

            HWID = CPUid;

            MD5 hashGenerator = MD5.Create();

            var hwidBytes = Encoding.ASCII.GetBytes(CPUid);
            var hashBytes = hashGenerator.ComputeHash(hwidBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)

            {

                sb.Append(hashBytes[i].ToString("X2"));

            }

            UNIQUE_IDENTITY = sb.ToString();
        }

        public string getHWID()
        {
            return HWID;
        }

        public string getUUID()
        {
            return UNIQUE_IDENTITY;
        }

    }
}
