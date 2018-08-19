using Matchmaker.Net.Client;

namespace Matchmaker.Net.Configuration
{
    public static class Debugging
    {
        public static bool DEBUG_ENABLED = false;
    }

    public static class SpamProtection
    {
        public static bool SPAM_PROTECTION_ENABLED = true;
        public static int FAILED_ATTEMPT_COUNT_MAX = 1;
    }

    public static class ServerVariables
    {
        public static int PORT = 25599,
                          MAX_CLIENTS_CONNECTED = 2, //change to -1 for no queue
                          BUFFER_SIZE = 2048,
                          CLIENT_CONNECTION_TIMEOUT = 6000; //ms until client is booted as inactive or assumed to fail transmission

        public static UUID IDENTITY; //this is automatically generated during runtime
    }
}
