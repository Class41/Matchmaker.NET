using Matchmaker.Net.Debug;
using Matchmaker.Net.Enums;
using Matchmaker.Net.Server;
using System.Threading;

namespace Matchmaker.Net.Network
{
    public class ClientQueueManager
    {
        private SocketManager _socketManager;

       public ClientQueueManager(SocketManager sockMgr)
        {
            this._socketManager = sockMgr;
            Thread asyncQueue = new Thread(BeginManager);
            asyncQueue.Start();
        }

        private void BeginManager()
        {
            while(true)
            {
                if(!ServerManager.ClientCanConnect() && ServerManager.GetOpenSlots() > 0)
                {
                    Logging.errlog("Releasing clients from connection queue", ErrorSeverity.ERROR_INFO);

                    for (int i = 1; i <= ServerManager.GetOpenSlots(); i++)
                    {
                        if (ServerManager.queuedClients.Count > 0)
                        {
                            DelayedQueueConnection connectionObject = ServerManager.queuedClients.Dequeue();
                            _socketManager.ReadAsyncDelayed(connectionObject.ar, connectionObject.clientState);
                            ServerManager.ConnectClient();
                        }
                    }
                }

                Thread.Sleep(250);
            }
        }
    }
}
