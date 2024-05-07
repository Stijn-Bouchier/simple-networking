using System.Diagnostics;

namespace SimpleServer
{
    internal class PingHandler : IMessageHandler
    {
        public const long ClientTimeout = 10000L; // 10 seconds

        private Server _simpleServer;
        private UdpServer _server;
        private ClientManager _clientManager;

        private static readonly byte[] PongData = DataParser.SerializeData(new ServerMessage() { Command = "pong" });

        public string Command => "ping";

        private Dictionary<string, Stopwatch> _timeScinceLastPing;

        public PingHandler(Server simpleServer, UdpServer server, ClientManager clientManager)
        {
            _simpleServer = simpleServer;
            _server = server;
            _clientManager = clientManager;

            _timeScinceLastPing = new Dictionary<string, Stopwatch>();

            simpleServer.ClientConnected += HandleClientConnected;
        }

        private void HandleClientConnected(string clientGuid)
        {
            _timeScinceLastPing.Add(clientGuid, new Stopwatch());
            _timeScinceLastPing[clientGuid].Start();
        }

        public void Handle(Message message)
        {
            Client client;

            if (_clientManager.TryGetClient(message.Guid, out client))
            {
                _server.SendData(client.Address, client.Port, PongData);

                _timeScinceLastPing[client.Guid].Restart();
            }
        }

        public void Tick()
        {
            List<string> disconnectedPlayerGuids = new List<string>(_timeScinceLastPing.Count);

            foreach (KeyValuePair<string, Stopwatch> kvp in _timeScinceLastPing)
            {
                if (kvp.Value.ElapsedMilliseconds >= ClientTimeout)
                {
                    disconnectedPlayerGuids.Add(kvp.Key);
                }
            }

            foreach (string playerGuid in disconnectedPlayerGuids)
            {
                _timeScinceLastPing.Remove(playerGuid);
                _clientManager.RemoveClient(playerGuid);

                byte[] connectionMessageData = DataParser.SerializeData(new ServerMessage() { Command = "disconnect", JsonString = playerGuid });

                foreach (string clientGuid in _clientManager.Clients)
                {
                    if (_clientManager.TryGetClient(clientGuid, out Client otherClient))
                    {
                        _server.SendData(otherClient.Address, otherClient.Port, connectionMessageData);
                    }
                }

                _simpleServer.TriggerClientDisconnected(playerGuid);
            }
        }
    }
}