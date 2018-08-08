using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Configuration;
using Matchmaker.Net.Debug;
using Matchmaker.Net.Client;

namespace Matchmaker.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration.Debugging.DEBUG_ENABLED = true;
            ServerManager.ServerManager.Launch();
        }
    }
}
