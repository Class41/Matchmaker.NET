using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Matchmaker.Net.Debug;
using Matchmaker.Net.Enums;
using Matchmaker.Net.Server;

namespace Matchmaker.Net.Network
{
    public class ClientQueueManager
    {
        SocketManager socketManager;

       public ClientQueueManager(SocketManager sockMgr)
        {
            this.socketManager = sockMgr;
            Thread asyncQueue = new Thread(beginManager);
            asyncQueue.Start();
        }

        private void beginManager()
        {
            while(true)
            {
                if(!ServerManager.clientCanConnect() && ServerManager.getOpenSlots() > 0)
                {
                    Logging.errlog("Releasing clients from connection queue", ErrorSeverity.ERROR_INFO);

                    for (int i = 1; i <= ServerManager.getOpenSlots(); i++)
                    {
                        DelayedQueueConnection connectionObject = ServerManager.queuedClients.Dequeue();
                        socketManager.readAsyncDelayed(connectionObject.ar, connectionObject.clientState);
                        ServerManager.connectClient();
                    }
                }

                Thread.Sleep(250);
            }
        }
    }
}
