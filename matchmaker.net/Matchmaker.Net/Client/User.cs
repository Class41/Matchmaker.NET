using Matchmaker.Net.Client;

namespace Matchmaker.Net.Matchmaker.Net.Client
{
    class UserEntry : ClientBase
    {
        public UserEntry()
        {
            _clientType = Enums.ClientType.CLIENT_TYPE_USER;
        }
    }
}
