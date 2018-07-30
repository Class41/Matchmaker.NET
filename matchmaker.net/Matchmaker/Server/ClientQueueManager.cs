using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matchmaker.Net.ServerManager;
using System.Threading;
using Matchmaker.Net.Debug;
using Matchmaker.Net.Enums;
using Matchmaker.Net.Network;

namespace Matchmaker.Net.Server
{
    public class ClientQueueManager
    {
        SocketManager socketManager;

       public ClientQueueManager(SocketManager sockMgr)
        {
            this.socketManager = sockMgr;
            beginManager();
        }

        private void beginManager()
        {
            while(true)
            {
                if(!ServerManager.ServerManager.clientCanConnect() && ServerManager.ServerManager.getOpenSlots() > 0)
                {
                    Logging.errlog("Releasing clients from connection queue", ErrorSeverity.ERROR_INFO);

                    socketManager.readAsyncDelayed();
                }

                Thread.Sleep(250);
            }
        }
    }
}
