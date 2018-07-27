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
        public int BUFFER_SIZE = 2048, requestBufferPosition = 0;
        public byte[] byteBuffer, requestBuffer;
        public StringBuilder data = new StringBuilder();

        public ServerConnectionStateObject()
        {
            byteBuffer = new Byte[BUFFER_SIZE];
            requestBuffer = new Byte[BUFFER_SIZE];
        }
    }
}
