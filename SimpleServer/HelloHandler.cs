using System.Text.Json;

namespace SimpleServer
{
    internal class HelloHandler : IMessageHandler
    {
        public string Command => "hello";

        private ClientManager _clientManager;
        private UdpServer _server;
        private Server _simpleServer;

        public HelloHandler(Server simpleServer, UdpServer server, ClientManager clientManager)
        {
            this._clientManager = clientManager;
            this._server = server;
            this._simpleServer = simpleServer;
        }

        public void Handle(Message message)
        {
            ClientHelloData helloData = JsonSerializer.Deserialize<ClientHelloData>(message.JsonData);

            Client client = new Client(message.ClientAddress, helloData.Port, helloData.ClientGuid);

            byte[] connectionMessageData = DataParser.SerializeData(new ServerMessage() { Command = "connect", JsonString = client.Guid });

            foreach (string clientGuid in _clientManager.Clients)
            {
                if (_clientManager.TryGetClient(clientGuid, out Client otherClient))
                {
                    _server.SendData(otherClient.Address, otherClient.Port, connectionMessageData);
                }
            }

            _clientManager.AddClient(client);
            _server.SendData(client.Address, client.Port, DataParser.SerializeData(new ServerMessage() { Command = "pong" }));

            _simpleServer.TriggerClientConnected(client.Guid);
        }

        public void Tick()
        {
            // Nothing to do
        }
    }
}