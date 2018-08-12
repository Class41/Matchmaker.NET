using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Configuration;
using Matchmaker.Net.Debug;
using Matchmaker.Net.Client;
using Matchmaker.Net.Server;
using Matchmaker.Net.Configuration.Sample;
using Matchmaker.Net.Configuration;

namespace Matchmaker.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration.Debugging.DEBUG_ENABLED = true;
            ServerManager.Launch(new ServerOperationDefinitionsSample()); //Override /Config/ServerOperationDefinitionsTemplate.cs or inherit from ServerOperation 
                                                                          //and implement and pass as an argument
        }
    }
}
