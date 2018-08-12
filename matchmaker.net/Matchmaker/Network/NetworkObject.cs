using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matchmaker.Net.Enums;

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
