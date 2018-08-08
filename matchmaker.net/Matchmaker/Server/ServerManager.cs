using System;
using System.Collections.Generic;

using Matchmaker.Net.Debug;
using Matchmaker.Net.Enums;
using Matchmaker.Net.Client;
using Matchmaker.Net.Network;
using Matchmaker.Net.Configuration;

namespace Matchmaker.Net.ServerManager
{
    public static class ServerManager
    {
        public static Queue<DelayedQueueConnection> queuedClients = new Queue<DelayedQueueConnection>();
        private static int currentlyOperatingClients = 0;

        public static bool clientCanConnect()
        {
            if (((currentlyOperatingClients < Configuration.ServerVariables.MAX_CLIENTS_CONNECTED) && queuedClients.Count == 0)  || Configuration.ServerVariables.MAX_CLIENTS_CONNECTED < 0)
                return true;
            else
                return false;
        }

        public static void connectClient()
        {
            currentlyOperatingClients++;
            Logging.errlog("Active clients updated: " + currentlyOperatingClients + "/" + Configuration.ServerVariables.MAX_CLIENTS_CONNECTED, ErrorSeverity.ERROR_INFO);
        }

        public static void diconnectClient()
        {
            currentlyOperatingClients--;
            Logging.errlog("Active clients updated: " + currentlyOperatingClients + "/" + Configuration.ServerVariables.MAX_CLIENTS_CONNECTED, ErrorSeverity.ERROR_INFO);
        }

        public static int getOpenSlots()
        {
            return Configuration.ServerVariables.MAX_CLIENTS_CONNECTED - currentlyOperatingClients;
        }

        public static void Launch()
        {
            Configuration.ServerVariables.IDENTITY = new UUID();
            SocketManager serverSocket = new SocketManager(Configuration.ServerVariables.PORT, new ServerOperationDefinitions());
        }
    }
}