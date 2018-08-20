using Matchmaker.Net.Client;
using Matchmaker.Net.Network;
using System.Collections.Generic;

namespace Matchmaker.Net.Server
{
    public static class RegisteredServerList
    {
        public static Dictionary<UUID, ServerEntry> registeredServers = new Dictionary<UUID, ServerEntry>();

        public static void RegisterServer(NetworkObject recievedObj)
        {
            registeredServers.Add(recievedObj.clientDetails._identity, (ServerEntry)recievedObj.clientDetails);
        }

        public static void UnregisterServer(NetworkObject recievedObj)
        {
            registeredServers.Remove(recievedObj.clientDetails._identity);
        }
    }
}
