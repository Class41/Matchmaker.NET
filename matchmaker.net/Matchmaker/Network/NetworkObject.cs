using Matchmaker.Net.Enums;
using System;

namespace Matchmaker.Net.Network
{
    [Serializable]
    public class NetworkObject
    {
        public string data;
        public NetObjectType requestType;

        public NetworkObject(NetObjectType type)
        {
            requestType = type;
        }
    }
}
