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
        public abstract void handleServerListRequest(ServerConnectionStateObject connection, NetworkObject obj);
        public abstract void handleModifyExistingServerRequest(ServerConnectionStateObject connection, NetworkObject obj);
        public abstract void handleRegisterNewServer(ServerConnectionStateObject connection, NetworkObject obj);
        public abstract void handleRespondToClient(ServerConnectionStateObject connection, NetworkObject obj);
        public abstract void handleMatchmakingRequest(ServerConnectionStateObject connection, NetworkObject obj);
        public abstract void handleUnregisterServerRequest(ServerConnectionStateObject connection, NetworkObject obj);

        protected void sendResponse(ServerConnectionStateObject connection, NetworkObject obj)
        {
            SocketManager.respondToClient(connection, obj);
        }
    }
}
