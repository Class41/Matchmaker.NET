using Matchmaker.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matchmaker.Net.Server
{
    public static class RegisteredServerList
    {
        public static Dictionary<UUID, ServerEntry> registeredServers = new Dictionary<UUID, ServerEntry>();
    }
}
