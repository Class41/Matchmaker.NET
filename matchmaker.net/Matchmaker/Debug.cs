using System;

namespace Matchmaker.Net.Debug
{
    public static class Debug
    {
        public static void dbgMessage<T>(T data)
        {
            dbgMessage("Data: " + data);
        }

        public static void dbgMessage(string message)
        {
            Console.WriteLine("\n" + message);
        }

        public static void dbgMessage<T>(string message, T data)
        {
            dbgMessage(message + ": " + data);
        }
    }
}