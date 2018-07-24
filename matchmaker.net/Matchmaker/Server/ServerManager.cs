using System;
using System.Collections.Generic;

using Matchmaker.Net.Debug;
using Matchmaker.Net.Enums;
using Matchmaker.Net.Client;

namespace Matchmaker.Net.ServerManager
{
    public static class ServerManager
    {
        private static Queue<Client.Client> clients = new Queue<Client.Client>();
        private static int MAX_CLIENTS_CONNECTED;
        private static int currentlyOperatingClients { get; set; }

        public static int GetCurrentlyOperatingClients()
        {
            return currentlyOperatingClients;
        }

        public static void Launch()
        {

        }
    }
}