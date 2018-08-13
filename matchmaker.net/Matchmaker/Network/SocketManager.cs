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
        
        private IPHostEntry _ipHostInfo;
        private IPAddress _ipAddress;
        private IPEndPoint _localEndPoint;
        private Socket _connectionSocket;
        private ServerOperation _opDef;
        public static ManualResetEvent threadFinished = new ManualResetEvent(false);

        public SocketManager(int port, ServerOperation operationDefinition)
        {
            ClientQueueManager queueManager = new ClientQueueManager(this);
            _opDef = operationDefinition;

            BeginListen(port);
        }

        private void BeginListen(int port)
        { 
            _ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            _ipAddress = _ipHostInfo.AddressList[1];
            _localEndPoint = new IPEndPoint(_ipAddress, port);
            _connectionSocket = new Socket(_ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            BindToPort();
        }

        private void BindToPort()
        {
            try
            {
                _connectionSocket.Bind(_localEndPoint);
                _connectionSocket.Listen(50);
            }
            catch (Exception e)
            {
                Debug.Logging.errlog("Cannot bind to port:\n" + e.Message + "\n" + e.StackTrace, ErrorSeverity.ERROR_CRITICAL_FAILURE);
                return;
            }

            ServerAsyncListen();
        }

        private void ServerAsyncListen()
        {
            Debug.Logging.errlog("Starting async listen on specified port", ErrorSeverity.ERROR_INFO);
            while (true)
            {
                threadFinished.Reset();
                _connectionSocket.BeginAccept(new AsyncCallback(HandleIncomingConnection), _connectionSocket);
                threadFinished.WaitOne();
            }
        }

        private void HandleIncomingConnection(IAsyncResult ar)
        {
            threadFinished.Set();

            Socket listener = (Socket) ar.AsyncState,
                   handler = listener.EndAccept(ar);

            ServerConnectionStateObject clientState = new ServerConnectionStateObject();
            clientState.workSocket = handler;

            IPEndPoint remoteIP = (IPEndPoint)handler.RemoteEndPoint;
            clientState.endpointIP = remoteIP.Address.ToString();
            clientState.endpointPort = remoteIP.Port.ToString();

            if (Configuration.SpamProtection.SPAM_PROTECTION_ENABLED)
                if (!AntispamProtection.CheckUser(clientState.endpointIP))
                    return;


            Debug.Logging.errlog(Utils.connectionInfo(clientState) +"Handling incoming connection request", ErrorSeverity.ERROR_INFO);

            if (ServerManager.ClientCanConnect())
            {
                ServerManager.ConnectClient();
                ReadAsyncDelayed(ar, clientState);
            }
            else
            {
                Debug.Logging.errlog(Utils.connectionInfo(clientState) + "CPU full: offloading incoming request to queue", ErrorSeverity.ERROR_INFO);
                ServerManager.queuedClients.Enqueue(new DelayedQueueConnection(ar, clientState));
            }
        }

        public void ReadAsyncDelayed(IAsyncResult result, ServerConnectionStateObject clientState)
        {
            try
            {
                clientState.workSocket.BeginReceive(clientState.byteBuffer, 0, Configuration.ServerVariables.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReadAsyncBytes), clientState);
            }
            catch(Exception e)
            {
                Debug.Logging.errlog(Utils.connectionInfo(clientState) + "Socket read failure:\n" + e.Message + "\n" + e.StackTrace, ErrorSeverity.ERROR_INFO);
                ShutdownAndCloseSocket(clientState);
                return;
            }
        }

        private void ReadAsyncBytes(IAsyncResult result)
        {
            ServerConnectionStateObject clientState = (ServerConnectionStateObject)result.AsyncState;
            try
            {
                Debug.Logging.errlog(Utils.connectionInfo(clientState) + "Reading data from socket", ErrorSeverity.ERROR_INFO);
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
                            Logging.errlog(recievedObject.data, ErrorSeverity.ERROR_INFO);
                            DecodeOperation(recievedObject, clientState);
                            return;
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.Logging.errlog(Utils.connectionInfo(clientState) + "Malformed or incomplete data, object conversion error:\n" + e.Message + "\n" + e.StackTrace, ErrorSeverity.ERROR_INFO);

                        if (Configuration.SpamProtection.SPAM_PROTECTION_ENABLED)
                            AntispamProtection.MarkForMaloformedData(clientState.endpointIP);

                        ShutdownAndCloseSocket(clientState);
                        return;
                    }
                }

                Array.ConstrainedCopy(clientState.byteBuffer, 0, clientState.requestBuffer, clientState.requestBufferPosition, bytecount);
                Array.Clear(clientState.byteBuffer, 0, Configuration.ServerVariables.BUFFER_SIZE);
                clientState.requestBufferPosition += bytecount;
                Debug.Logging.errlog(Utils.connectionInfo(clientState) + "Recieved " + bytecount + " bytes (" + clientState.requestBufferPosition + " total bytes stored in instance)", ErrorSeverity.ERROR_INFO);
                Debug.Logging.dbgMessageByteArray<byte>(clientState.requestBuffer);

                clientState.workSocket.BeginReceive(clientState.byteBuffer, 0, Configuration.ServerVariables.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReadAsyncBytes), clientState);
            }
            catch (Exception e)
            {
                Debug.Logging.errlog(Utils.connectionInfo(clientState) + "Something went wrong reading from socket:\n" + e.Message + "\n" + e.StackTrace, ErrorSeverity.ERROR_WARNING);
                if (Configuration.SpamProtection.SPAM_PROTECTION_ENABLED)
                    AntispamProtection.MarkForMaloformedData(clientState.endpointIP);
                ShutdownAndCloseSocket(clientState);
                return;
            }
        }

        private void DecodeOperation(NetworkObject recievedNetworkObject, ServerConnectionStateObject clientState)
        {
            try
            {
                switch (recievedNetworkObject.requestType)
                {
                    case NetObjectType.CLIENT_REQUEST_SERVER_LIST:
                        _opDef.HandleServerListRequest(clientState, recievedNetworkObject);
                        break;
                    case NetObjectType.CLIENT_SERVER_MODIFY_REGISTERED_SERVER:
                        _opDef.HandleModifyExistingServerRequest(clientState, recievedNetworkObject);
                        break;
                    case NetObjectType.CLIENT_SERVER_REGISTER_SERVER:
                        _opDef.HandleRegisterNewServer(clientState, recievedNetworkObject);
                        break;
                    case NetObjectType.CLIENT_SERVER_RESPONSE_GENERIC:
                        _opDef.HandleRespondToClient(clientState, recievedNetworkObject);
                        break;
                    case NetObjectType.CLIENT_SERVER_UNREGISTER_SERVER:
                        _opDef.HandleUnregisterServerRequest(clientState, recievedNetworkObject);
                        break;
                    case NetObjectType.SERVER_SEND_MATCHMAKE:
                        _opDef.HandleMatchmakingRequest(clientState, recievedNetworkObject);
                        break;
                }
            }
            catch(Exception e)
            {
                Debug.Logging.errlog(Utils.connectionInfo(clientState) + "Something went wrong when reading data!\n" + e.Message + "\n" + e.StackTrace, ErrorSeverity.ERROR_INFO);
                ShutdownAndCloseSocket(clientState);
                return;
            }
        }

        public static void RespondToClient(ServerConnectionStateObject clientState, NetworkObject obj)
        {
            try
            {
                byte[] networkObjectBytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));

                clientState.workSocket.BeginSend(networkObjectBytes, 0, networkObjectBytes.Length, 0,
                                                new AsyncCallback(ClientResponseStatus), clientState);
            }
            catch (Exception e)
            {
                Debug.Logging.errlog(Utils.connectionInfo(clientState) + "Unable to send client data:\n" + e.Message + "\n" + e.StackTrace, ErrorSeverity.ERROR_INFO);
                ShutdownAndCloseSocket(clientState);
                return;
            }
        }

        public static void ClientResponseStatus(IAsyncResult result)
        {
            ServerConnectionStateObject clientState = (ServerConnectionStateObject)result.AsyncState;
            try
            {
                int responseSize = clientState.workSocket.EndSend(result);
                Debug.Logging.errlog(Utils.connectionInfo(clientState) + "Sent data to client: " + responseSize + " Bytes.", ErrorSeverity.ERROR_INFO);
                ShutdownAndCloseSocket(clientState);
            }
            catch (Exception e)
            {
                Debug.Logging.errlog(Utils.connectionInfo(clientState) + "Unable to send client data:\n" + e.Message + "\n" + e.StackTrace, ErrorSeverity.ERROR_INFO);
                ShutdownAndCloseSocket(clientState);
                return;
            }
        }

        public static void ShutdownAndCloseSocket(ServerConnectionStateObject clientState)
        {
            try
            {
                if (clientState.workSocket.Connected)
                {
                    ServerManager.DiconnectClient();
                    clientState.workSocket.Shutdown(SocketShutdown.Both);
                    clientState.workSocket.Close();
                }
                else
                {
                    ServerManager.DiconnectClient();
                }
            }
            catch (Exception e)
            {
                Debug.Logging.errlog(Utils.connectionInfo(clientState) + "Failed to shutdown client:\n" + e.Message + "\n" + e.StackTrace, ErrorSeverity.ERROR_INFO);
                return;
            }
        }
    }
}
