using Matchmaker.Net.Client;
using Matchmaker.Net.Enums;
using System;

namespace Matchmaker.Net.Network
{
    [Serializable]
    public class NetworkObject
    {
        public ClientBase clientDetails;
        public NetObjectType requestType;

        public NetworkObject(NetObjectType type)
        {
            requestType = type;
        }
    }
}
