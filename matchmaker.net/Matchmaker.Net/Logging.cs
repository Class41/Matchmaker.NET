using System;
using Matchmaker.Net.Enums;
using Matchmaker.Net.Configuration;
using Matchmaker.Net.Network;

namespace Matchmaker.Net.Debug
{
    public static class Utils
    {
        public static string connectionInfo(ServerConnectionStateObject clientState)
        {
            return "(" + clientState.endpointIP + ":" + clientState.endpointPort + ") ";
        }
    }

    public static class Logging
    {
        public static void dbgMessage<T>(T data)
        {
            dbgMessage("Data: " + data);
        }

        public static void dbgMessage(string message)
        {
            if (Configuration.Debugging.DEBUG_ENABLED)
                Console.WriteLine(message);
        }

        public static void dbgMessage<T>(string message, T data)
        {
            dbgMessage(message + ": " + data);
        }

        public static void dbgMessageArray<T>(T[] data)
        {
            int position = 0;
            foreach (T value in data)
            {
                dbgMessage(position++ + " : " + value.ToString());
            }
        }

        public static void dbgMessageByteArray<T>(T[] data)
        {
            int position = 0;
            foreach (T value in data)
            {
                if (value.ToString() != "0")
                    dbgMessage(position++ + " : " + value.ToString());
            }
        }

        public static void dbgMessageByteArrayRaw<T>(T[] data)
        {
            int position = 0;
            foreach (T value in data)
            {
                dbgMessage(position++ + " : " + value.ToString());
            }
        }

        public static void dbgMessageByteArrayDiff<T>(T[] data, T[] data2)
        {
            int shortest = -1;
            if (data.Length == data2.Length)
            {
                dbgMessage("Arrays are same size...");
                shortest = data.Length;
            }
            else if (data.Length > data2.Length)
            {
                dbgMessage("Input 1 larger than input 2");
                shortest = data2.Length;
            }
            else
            {
                dbgMessage("Input 2 larger than input 1");
                shortest = data.Length;
            }

            for (int i = 0; i < shortest; i++)
            {
                if (data[i].ToString() != data2[i].ToString())
                    dbgMessage("Difference found at position " + i + " :: " + data[i].ToString() + " vs " + data2[i].ToString());
            }
        }

        public static void errlog(string message, ErrorSeverity data)
        {
            switch (data)
            {
                case ErrorSeverity.ERROR_INFO:
                    dbgMessage("[INFO] :: " + message);
                    break;
                case ErrorSeverity.ERROR_WARNING:
                    dbgMessage("[WARN] :: " + message);
                    break;
                case ErrorSeverity.ERROR_ATTENTION:
                    dbgMessage("[ATTN] :: " + message);
                    break;
                case ErrorSeverity.ERROR_CRITICAL_FAILURE:
                    dbgMessage("[FAIL] :: " + message);
                    break;
            }
        }
    }
}