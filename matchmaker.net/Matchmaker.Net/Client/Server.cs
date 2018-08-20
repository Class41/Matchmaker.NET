using System;

namespace Matchmaker.Net.Client
{
    [Serializable]
    public class ServerEntry : ClientBase
    {
        public string _serverIP,
                      _serverPort,
                      _customMessage,
                      _type = "default",
                      _gameMode;

        public int _maxPlayers,
                   _currentPlayers;

        public ServerEntry()
        {
            _clientType = Enums.ClientType.CLIENT_TYPE_SERVER;
        }
    }
}