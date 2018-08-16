using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Matchmaker.Net.Network
{
    public class ServerConnectionStateObject
    {
        public Socket workSocket = null;
        public int requestBufferPosition = 0;
        public byte[] byteBuffer, requestBuffer;
        public StringBuilder data = new StringBuilder();
        public string endpointIP, endpointPort;
        public Timer timeoutTimer;
        public bool disconnectCounted = false;

        public ServerConnectionStateObject()
        {
            byteBuffer = new Byte[Configuration.ServerVariables.BUFFER_SIZE];
            requestBuffer = new Byte[Configuration.ServerVariables.BUFFER_SIZE];

            timeoutTimer = new Timer(new TimerCallback(SocketTimedOut), null, Timeout.Infinite, Timeout.Infinite);
        }

        private void SocketTimedOut(object state)
        {
            SocketManager.ShutdownAndCloseSocket(this);
        }
    }
}
