namespace Matchmaker.Net.Client
{
    public class ServerEntry
    {
        public UUID _identity;
        public string _serverIP,
                      _serverPort,
                      _customMessage,
                      _type = "default",
                      _gameMode;
        public int _maxPlayers,
                   _currentPlayers;
    }
}