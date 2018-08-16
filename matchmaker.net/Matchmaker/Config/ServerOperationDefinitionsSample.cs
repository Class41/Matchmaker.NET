using Matchmaker.Net.Network;
using Matchmaker.Net.Server;
using System;

namespace Matchmaker.Net.Configuration.Sample
{
    public class ServerOperationDefinitionsSample : ServerOperation
    {

        //Called when client makes a matchmaking request
        public override void HandleMatchmakingRequest(ServerConnectionStateObject connection, NetworkObject recievedObj)
        {
            NetworkObject response = new NetworkObject(recievedObj.requestType);
            throw new NotImplementedException(); //remove this and replace with functionality
            SendResponse(connection, response);
        }

        //called when the client tries to modify existing registered server in server list
        public override void HandleModifyExistingServerRequest(ServerConnectionStateObject connection, NetworkObject recievedObj)
        {
            NetworkObject response = new NetworkObject(recievedObj.requestType);
            throw new NotImplementedException(); //remove this and replace with functionality
            SendResponse(connection, response);
        }

        //called when client asks to register a new server
        public override void HandleRegisterNewServer(ServerConnectionStateObject connection, NetworkObject recievedObj)
        {
            NetworkObject response = new NetworkObject(recievedObj.requestType);
            throw new NotImplementedException(); //remove this and replace with functionality
            SendResponse(connection, response);
        }

        //called when a generic response is required
        public override void HandleRespondToClient(ServerConnectionStateObject connection, NetworkObject recievedObj)
        {
           NetworkObject response = new NetworkObject(recievedObj.requestType);
            response.data = "@SeverEcho: " + recievedObj.data;
           //throw new NotImplementedException(); //remove this and replace with functionality
           SendResponse(connection, response);
        }

        //called when the client requests the server list
        public override void HandleServerListRequest(ServerConnectionStateObject connection, NetworkObject recievedObj)
        {
            NetworkObject response = new NetworkObject(recievedObj.requestType);
            throw new NotImplementedException(); //remove this and replace with functionality
            SendResponse(connection, response);
        }

        //called when the client asks to unregister a currently registered server
        public override void HandleUnregisterServerRequest(ServerConnectionStateObject connection, NetworkObject recievedObj)
        {
            NetworkObject response = new NetworkObject(recievedObj.requestType);
            throw new NotImplementedException(); //remove this and replace with functionality
            SendResponse(connection, response);
        }
    }
}
