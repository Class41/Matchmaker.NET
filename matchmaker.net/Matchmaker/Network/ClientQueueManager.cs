using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matchmaker.Net.ServerManager;
using System.Threading;
using Matchmaker.Net.Debug;
using Matchmaker.Net.Enums;

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
                if(!ServerManager.ServerManager.clientCanConnect() && ServerManager.ServerManager.getOpenSlots() > 0)
                {
                    Logging.errlog("Releasing clients from connection queue", ErrorSeverity.ERROR_INFO);

                    for (int i = 1; i <= ServerManager.ServerManager.getOpenSlots(); i++)
                    {
                        DelayedQueueConnection connectionObject = ServerManager.ServerManager.queuedClients.Dequeue();
                        socketManager.readAsyncDelayed(connectionObject.ar, connectionObject.clientState);
                        ServerManager.ServerManager.connectClient();
                    }
                }

                Thread.Sleep(250);
            }
        }
    }
}
