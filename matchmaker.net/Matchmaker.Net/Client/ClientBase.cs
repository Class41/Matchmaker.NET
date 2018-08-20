using Matchmaker.Net.Enums;
using System;

namespace Matchmaker.Net.Client
{
    [Serializable]
    public class ClientBase
    {
        public UUID _identity;
        public ClientType _clientType;
    }
}
