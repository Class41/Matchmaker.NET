using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matchmaker.Net.Enums
{
    public enum NetObjectType
    {
        CLIENT_HEARTBEAT,
        CLIENT_REQUEST_MATCHMAKE,
        CLIENT_REQUEST_UNMATCHMAKE,

        CLIENT_SERVER_HEARTBEAT,
        CLIENT_SERVER_REGISTER_SERVER,
        CLIENT_SERVER_UNREGISTER_SERVER,
        CLIENT_SERVER_MODIFY_REGISTERED_SERVER,

        SERVER_SEND_MATCHMAKE,
        SERVER_SEND_STATUS_RESPONSE
    }
}
