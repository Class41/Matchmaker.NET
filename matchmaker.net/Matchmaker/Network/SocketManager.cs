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
using System.IO;

namespace Matchmaker.Net.Network
{
    class SocketManager
    {
        public SocketManager(int port) { BeginListen(port); }
        
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
            clientState.workSocket.BeginReceive(clientState.byteBuffer,0, clientState.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(readAsyncBytes), clientState);
        }

        private void readAsyncBytes(IAsyncResult ar)
        {
            try
            {
                Debug.Logging.errlog("Reading data from socket", ErrorSeverity.ERROR_INFO);
                ServerConnectionStateObject clientState = (ServerConnectionStateObject)ar.AsyncState;
                int bytecount = clientState.workSocket.EndReceive(ar);
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
