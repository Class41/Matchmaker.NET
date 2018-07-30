using System;
using System.Collections.Generic;

using Matchmaker.Net.Debug;
using Matchmaker.Net.Enums;
using Matchmaker.Net.Client;
using Matchmaker.Net.Network;

namespace Matchmaker.Net.ServerManager
{
    public static class ServerManager
    {
        public static Queue<DelayedQueueConnection> queuedClients = new Queue<DelayedQueueConnection>();
        private static int MAX_CLIENTS_CONNECTED = 2;
        private static int currentlyOperatingClients = 0;
        private static int SERVER_INCOMING_OUTGOING_CONNECTION_PORT = 25599;
        private static UUID IDENTITY;

        public static bool clientCanConnect()
        {
            if (((currentlyOperatingClients < MAX_CLIENTS_CONNECTED) && queuedClients.Count == 0)  || MAX_CLIENTS_CONNECTED < 0)
                return true;
            else
                return false;
        }

        public static void connectClient()
        {
            currentlyOperatingClients++;
            Logging.errlog("Active clients updated: " + currentlyOperatingClients + "/" + MAX_CLIENTS_CONNECTED, ErrorSeverity.ERROR_INFO);
        }

        public static void diconnectClient()
        {
            currentlyOperatingClients--;
            Logging.errlog("Active clients updated: " + currentlyOperatingClients + "/" + MAX_CLIENTS_CONNECTED, ErrorSeverity.ERROR_INFO);
        }

        public static int getOpenSlots()
        {
            return MAX_CLIENTS_CONNECTED - currentlyOperatingClients;
        }

        public static void Launch()
        {
            IDENTITY = new UUID();
            SocketManager serverSocket = new SocketManager(SERVER_INCOMING_OUTGOING_CONNECTION_PORT);
        }
    }
}