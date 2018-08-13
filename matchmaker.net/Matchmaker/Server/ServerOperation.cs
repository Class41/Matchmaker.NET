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
        public abstract void HandleServerListRequest(ServerConnectionStateObject connection, NetworkObject obj);
        public abstract void HandleModifyExistingServerRequest(ServerConnectionStateObject connection, NetworkObject obj);
        public abstract void HandleRegisterNewServer(ServerConnectionStateObject connection, NetworkObject obj);
        public abstract void HandleRespondToClient(ServerConnectionStateObject connection, NetworkObject obj);
        public abstract void HandleMatchmakingRequest(ServerConnectionStateObject connection, NetworkObject obj);
        public abstract void HandleUnregisterServerRequest(ServerConnectionStateObject connection, NetworkObject obj);

        protected void SendResponse(ServerConnectionStateObject connection, NetworkObject obj)
        {
            SocketManager.RespondToClient(connection, obj);
        }
    }
}
