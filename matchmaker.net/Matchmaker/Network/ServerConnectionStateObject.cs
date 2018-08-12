using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Matchmaker.Net.Network
{
    public class ServerConnectionStateObject
    {
        public Socket workSocket = null;
        public int requestBufferPosition = 0;
        public byte[] byteBuffer, requestBuffer;
        public StringBuilder data = new StringBuilder();
        public string endpointIP, endpointPort;

        public ServerConnectionStateObject()
        {
            byteBuffer = new Byte[Configuration.ServerVariables.BUFFER_SIZE];
            requestBuffer = new Byte[Configuration.ServerVariables.BUFFER_SIZE];
        }
    }
}
