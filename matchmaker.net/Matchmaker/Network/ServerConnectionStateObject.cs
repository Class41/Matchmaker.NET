using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Matchmaker.Net.Network
{
    class ServerConnectionStateObject
    {
        public Socket workSocket = null;
        public const int BUFFER_SIZE = 2048;
        public byte[] byteBuffer = new Byte[BUFFER_SIZE];
        public StringBuilder data = new StringBuilder();
    }
}
