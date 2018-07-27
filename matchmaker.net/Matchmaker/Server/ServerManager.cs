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
        private static Queue<Client.Client> clients = new Queue<Client.Client>();
        private static int MAX_CLIENTS_CONNECTED;
        private static int currentlyOperatingClients { get; set; }
        private static int SERVER_INCOMING_OUTGOING_CONNECTION_PORT = 25599;
             
        public static int GetCurrentlyOperatingClients()
        {
            return currentlyOperatingClients;
        }

        public static void Launch()
        {
            SocketManager serverSocket = new SocketManager(SERVER_INCOMING_OUTGOING_CONNECTION_PORT);
        }
    }
}