using System;

namespace Matchmaker.Net.Network
{
    public class DelayedQueueConnection
    {
        public IAsyncResult ar;
        public ServerConnectionStateObject clientState;

        public DelayedQueueConnection(IAsyncResult ar, ServerConnectionStateObject clientState)
        {
            this.ar = ar;
            this.clientState = clientState;
        }
    }
}
