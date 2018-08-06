using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Matchmaker.Net.Debug;
using Matchmaker.Net.Enums;
using Matchmaker.Net.ServerManager;
using System.Threading;
using Matchmaker.Net.Network;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Security;
using System.IO;
using Matchmaker.Net.Server;

namespace Matchmaker.Net.Network
{
    public class SocketManager
    {
        public SocketManager(int port) { ClientQueueManager queueManager = new ClientQueueManager(this);  BeginListen(port);  }
        
        private IPHostEntry ipHostInfo;
        private IPAddress ipAddress;
        private IPEndPoint localEndPoint;
        private Socket connectionSocket;
        public static ManualResetEvent threadFinished = new ManualResetEvent(false);

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

            if (ServerManager.ServerManager.clientCanConnect())
            {
                Debug.Logging.errlog("Directly handling incoming request", ErrorSeverity.ERROR_INFO);
                ServerManager.ServerManager.connectClient();
                readAsyncDelayed(ar, clientState);
            }
            else
            {
                Debug.Logging.errlog("CPU full: offloading incoming request to queue", ErrorSeverity.ERROR_INFO);
                ServerManager.ServerManager.queuedClients.Enqueue(new DelayedQueueConnection(ar, clientState));
            }
        }

        public void readAsyncDelayed(IAsyncResult ar, ServerConnectionStateObject clientState)
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

        private void readAsyncBytes(IAsyncResult ar)
        {
            try
            {
                Debug.Logging.errlog("Reading data from socket", ErrorSeverity.ERROR_INFO);
                ServerConnectionStateObject clientState = (ServerConnectionStateObject)ar.AsyncState;
                int bytecount = clientState.workSocket.EndReceive(ar);

                if(bytecount == 5)
                {
                    try
                    {
                        string eofCheck = Encoding.ASCII.GetString(clientState.byteBuffer);
                        if (eofCheck.IndexOf("<EOF>") != -1)
                        {
                            decodeOperation((NetworkObject)ByteArrayToObject(clientState.requestBuffer), clientState);
                            ServerManager.ServerManager.diconnectClient();
                            return;
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.Logging.errlog("Malformed or incomplete data, object conversion error", ErrorSeverity.ERROR_INFO);
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
                ServerManager.ServerManager.diconnectClient();
            }
        }

        private void decodeOperation(NetworkObject networkObject, ServerConnectionStateObject clientState)
        {
            switch(networkObject.requestType)
            {
                case NetObjectType.CLIENT_REQUEST_SERVER_LIST:
                    break;
                case NetObjectType.CLIENT_SERVER_MODIFY_REGISTERED_SERVER:
                    break;
                case NetObjectType.CLIENT_SERVER_REGISTER_SERVER:
                    break;
                case NetObjectType.CLIENT_SERVER_RESPONSE_GENERIC:
                    break;
                case NetObjectType.CLIENT_SERVER_UNREGISTER_SERVER:
                    break;
                case NetObjectType.SERVER_SEND_MATCHMAKE:
                    break;
            }
        }

        public byte[] objectToByteArray(object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        private Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }
    }
}
