using Matchmaker.Net.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matchmaker.Net.Network;

namespace Matchmaker.Net.Configuration
{
    class ServerOperationDefinitions : ServerOperation
    {
        public ServerOperationDefinitions(ServerConnectionStateObject connection, NetworkObject obj) : base(connection, obj)
        {
        }

        public override void handleMatchmakingRequest()
        {
            throw new NotImplementedException();
        }

        public override void handleModifyExistingServerRequest()
        {
            throw new NotImplementedException();
        }

        public override void handleRegisterNewServer()
        {
            throw new NotImplementedException();
        }

        public override void handleRespondToClient()
        {
            throw new NotImplementedException();
        }

        public override void handleServerListRequest()
        {
            throw new NotImplementedException();
        }

        public override void handleUnregisterServerRequest()
        {
            throw new NotImplementedException();
        }
    }
}
