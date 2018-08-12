using Matchmaker.Net.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matchmaker.Net.Network;

namespace Matchmaker.Net.Configuration
{
    public class ServerOperationDefinitions : ServerOperation
    {
        public void test()
        {
            Console.WriteLine("Completed test");
        }

        //Called when client makes a matchmaking request
        public override void handleMatchmakingRequest(ServerConnectionStateObject connection, NetworkObject recievedObj)
        {
            NetworkObject response = new NetworkObject(recievedObj.requestType);
            throw new NotImplementedException(); //remove this and replace with functionality
            sendResponse(connection, response);
        }

        //called when the client tries to modify existing registered server in server list
        public override void handleModifyExistingServerRequest(ServerConnectionStateObject connection, NetworkObject recievedObj)
        {
            NetworkObject response = new NetworkObject(recievedObj.requestType);
            throw new NotImplementedException(); //remove this and replace with functionality
            sendResponse(connection, response);
        }

        //called when client asks to register a new server
        public override void handleRegisterNewServer(ServerConnectionStateObject connection, NetworkObject recievedObj)
        {
            NetworkObject response = new NetworkObject(recievedObj.requestType);
            throw new NotImplementedException(); //remove this and replace with functionality
            sendResponse(connection, response);
        }

        //called when a generic response is required
        public override void handleRespondToClient(ServerConnectionStateObject connection, NetworkObject recievedObj)
        {
           NetworkObject response = new NetworkObject(recievedObj.requestType);
           throw new NotImplementedException(); //remove this and replace with functionality
           sendResponse(connection, response);
        }

        //called when the client requests the server list
        public override void handleServerListRequest(ServerConnectionStateObject connection, NetworkObject recievedObj)
        {
            NetworkObject response = new NetworkObject(recievedObj.requestType);
            throw new NotImplementedException(); //remove this and replace with functionality
            sendResponse(connection, response);
        }

        //called when the client asks to unregister a currently registered server
        public override void handleUnregisterServerRequest(ServerConnectionStateObject connection, NetworkObject recievedObj)
        {
            NetworkObject response = new NetworkObject(recievedObj.requestType);
            throw new NotImplementedException(); //remove this and replace with functionality
            sendResponse(connection, response);
        }
    }
}
