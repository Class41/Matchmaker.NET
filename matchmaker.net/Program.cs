using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Matchmaker.Net.Debug;
using Matchmaker.Net.Client;

namespace Matchmaker.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Logging.MODE_DEBUG = true;
            ServerManager.ServerManager.Launch();
        }
    }
}
