using System;
using Matchmaker.Net.Enums;

namespace Matchmaker.Net.Debug
{
    public static class Logging
    {
        public static bool MODE_DEBUG = false;

        public static void dbgMessage<T>(T data)
        {
            dbgMessage("Data: " + data);
        }

        public static void dbgMessage(string message)
        {
            if (MODE_DEBUG)
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
                if(value.ToString() != "0")
                dbgMessage(position++ + " : " + value.ToString());
            }
        }

        public static void errlog(string message, ErrorSeverity data)
        {
            switch(data)
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