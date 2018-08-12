using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Matchmaker.Net.Debug;
using Matchmaker.Net.Enums;
using System.Threading;
using Matchmaker.Net.Network;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Security;
using System.IO;
using Matchmaker.Net.Server;
using Newtonsoft.Json;
using Matchmaker.Net.Configuration;

namespace Matchmaker.Net.Network
{
    public class SocketManager
    {
        
        private IPHostEntry ipHostInfo;
        private IPAddress ipAddress;
        private IPEndPoint localEndPoint;
        private Socket connectionSocket;
        private ServerOperation opDef;
        public static ManualResetEvent threadFinished = new ManualResetEvent(false);

        public SocketManager(int port, ServerOperation operationDefinition)
        {
            ClientQueueManager queueManager = new ClientQueueManager(this);
            opDef = operationDefinition;

            BeginListen(port);
        }

        private void BeginListen(int port)
        { 
            ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            ipAddress = ipHostInfo.AddressList[1];
            localEndPoint = new IPEndPoint(ipAddress, port);
            connectionSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            bindToPort();
        }

        private void bindToPort()
        {
            try
            {
                connectionSocket.Bind(localEndPoint);
                connectionSocket.Listen(50);
            }
            catch (Exception e)
            {
                Debug.Logging.errlog("Cannot bind to port:\n" + e.StackTrace, ErrorSeverity.ERROR_CRITICAL_FAILURE);
            }

            serverAsyncListen();
        }

        private void serverAsyncListen()
        {
            Debug.Logging.errlog("Starting async listen on specified port", ErrorSeverity.ERROR_INFO);
            while (true)
            {
                threadFinished.Reset();
                connectionSocket.BeginAccept(new AsyncCallback(HandleIncomingConnection), connectionSocket);
                threadFinished.WaitOne();
            }
        }

        private void HandleIncomingConnection(IAsyncResult ar)
        {
            Debug.Logging.errlog("Handling incoming connection request", ErrorSeverity.ERROR_INFO);
            threadFinished.Set();

            Socket listener = (Socket) ar.AsyncState,
                   handler = listener.EndAccept(ar);

            ServerConnectionStateObject clientState = new ServerConnectionStateObject();
            clientState.workSocket = handler;

            if (ServerManager.clientCanConnect())
            {
                Debug.Logging.errlog("Directly handling incoming request", ErrorSeverity.ERROR_INFO);
                ServerManager.connectClient();
                readAsyncDelayed(ar, clientState);
            }
            else
            {
                Debug.Logging.errlog("CPU full: offloading incoming request to queue", ErrorSeverity.ERROR_INFO);
                ServerManager.queuedClients.Enqueue(new DelayedQueueConnection(ar, clientState));
            }
        }

        public void readAsyncDelayed(IAsyncResult result, ServerConnectionStateObject clientState)
        {
            try
            {
                clientState.workSocket.BeginReceive(clientState.byteBuffer, 0, clientState.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(readAsyncBytes), clientState);
            }
            catch(Exception e)
            {
                Debug.Logging.errlog("Socket read failure:\n" + e.StackTrace, ErrorSeverity.ERROR_INFO);
            }
        }

        private void readAsyncBytes(IAsyncResult result)
        {
            try
            {
                Debug.Logging.errlog("Reading data from socket", ErrorSeverity.ERROR_INFO);
                ServerConnectionStateObject clientState = (ServerConnectionStateObject)result.AsyncState;
                int bytecount = clientState.workSocket.EndReceive(result);

                if(bytecount == 5)
                {
                    try
                    {
                        string eofCheck = Encoding.ASCII.GetString(clientState.byteBuffer);
                        if (eofCheck.IndexOf("<EOF>") != -1)
                        {
                            byte[] extractedRecievedData = new byte[clientState.requestBufferPosition];
                            Array.Copy(clientState.requestBuffer, extractedRecievedData, clientState.requestBufferPosition);

                            NetworkObject recievedObject = JsonConvert.DeserializeObject<NetworkObject>(Encoding.ASCII.GetString(extractedRecievedData));
                            Console.WriteLine(recievedObject.data);

                            decodeOperation(recievedObject, clientState);
                            ServerManager.diconnectClient();
                            return;
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.Logging.errlog("Malformed or incomplete data, object conversion error:\n" + e.StackTrace, ErrorSeverity.ERROR_INFO);
                        return;
                    }
                }

                Array.ConstrainedCopy(clientState.byteBuffer, 0, clientState.requestBuffer, clientState.requestBufferPosition, bytecount);
                Array.Clear(clientState.byteBuffer, 0, clientState.BUFFER_SIZE);
                clientState.requestBufferPosition += bytecount;
                Debug.Logging.errlog("Recieved " + bytecount + " bytes (" + clientState.requestBufferPosition + " total bytes stored in instance)", ErrorSeverity.ERROR_INFO);
                Debug.Logging.dbgMessageByteArray<byte>(clientState.requestBuffer);

                clientState.workSocket.BeginReceive(clientState.byteBuffer, 0, clientState.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(readAsyncBytes), clientState);
            }
            catch (Exception e)
            {
                Debug.Logging.errlog("Something went wrong reading from socket:\n" + e.StackTrace, ErrorSeverity.ERROR_WARNING);
                ServerManager.diconnectClient();
            }
        }

        private void decodeOperation(NetworkObject recievedNetworkObject, ServerConnectionStateObject clientState)
        {
            //opDef = new ServerOperationDefinitions();

            switch(recievedNetworkObject.requestType)
            {
                case NetObjectType.CLIENT_REQUEST_SERVER_LIST:
                    opDef.handleServerListRequest(clientState, recievedNetworkObject);
                    break;
                case NetObjectType.CLIENT_SERVER_MODIFY_REGISTERED_SERVER:
                    opDef.handleModifyExistingServerRequest(clientState, recievedNetworkObject);
                    break;
                case NetObjectType.CLIENT_SERVER_REGISTER_SERVER:
                    opDef.handleRegisterNewServer(clientState, recievedNetworkObject);
                    break;
                case NetObjectType.CLIENT_SERVER_RESPONSE_GENERIC:
                    opDef.handleRespondToClient(clientState, recievedNetworkObject);
                    break;
                case NetObjectType.CLIENT_SERVER_UNREGISTER_SERVER:
                    opDef.handleUnregisterServerRequest(clientState, recievedNetworkObject);
                    break;
                case NetObjectType.SERVER_SEND_MATCHMAKE:
                    opDef.handleMatchmakingRequest(clientState, recievedNetworkObject);
                    break;
            }
        }

        public static void respondToClient(ServerConnectionStateObject connection, NetworkObject obj)
        {
            try
            {
                byte[] networkObjectBytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));

                connection.workSocket.BeginSend(networkObjectBytes, 0, connection.BUFFER_SIZE, 0,
                                                new AsyncCallback(clientResponseStatus), connection);
            }
            catch (Exception e)
            {
                Debug.Logging.errlog("Unable to send client data:\n" + e.StackTrace, ErrorSeverity.ERROR_INFO);
            }
        }

        public static void clientResponseStatus(IAsyncResult result)
        {
            Socket connection = (Socket)result.AsyncState;
            int responseSize = connection.EndSend(result);

            connection.Shutdown(SocketShutdown.Both);
            connection.Close();
        }
    }
}
