using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Matchmaker.Net.Client
{
    public class UUID
    {
        private string _UNIQUE_IDENTITY;
        private string _HWID;

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
