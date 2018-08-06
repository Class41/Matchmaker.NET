using Matchmaker.Net.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matchmaker.Net.Server
{
    public abstract class ServerOperation
    {
        ServerConnectionStateObject connectionInformation;
        NetworkObject recievedObject;

        public ServerOperation(ServerConnectionStateObject connection, NetworkObject obj)
        {
            connectionInformation = connection;
            recievedObject = obj;
        }

        public abstract void handleServerListRequest();
        public abstract void handleModifyExistingServerRequest();
        public abstract void handleRegisterNewServer();
        public abstract void handleRespondToClient();
        public abstract void handleMatchmakingRequest();
        public abstract void handleUnregisterServerRequest();


    }
}
