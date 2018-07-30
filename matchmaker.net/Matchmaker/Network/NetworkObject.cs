using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matchmaker.Net.Enums;

namespace Matchmaker.Net.Network
{
    [Serializable]
    class NetworkObject
    {
        NetObjectType requestType;

        public NetworkObject(NetObjectType type)
        {

        }
    }
}
