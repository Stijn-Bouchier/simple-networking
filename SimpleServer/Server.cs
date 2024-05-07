namespace SimpleServer
{
    public class Server
    {
        public delegate void ClientConnectedDelegate(string clientGuid);
        public delegate void ClientDisconnectedDelegate(string clientGuid);
        public delegate void MessageReceivedDelegate(string clientGuid, string message);

        /// <summary>
        /// Event that get's called when data is sent from a client to the server.
        /// </summary>
        public event MessageReceivedDelegate? MessageReceived;
        /// <summary>
        /// Event that gets called when a client is connected to the server.
        /// </summary>
        public event ClientConnectedDelegate? ClientConnected;
        /// <summary>
        /// Event that gets called when a client get's disconnected from the server.
        /// </summary>
        public event ClientDisconnectedDelegate? ClientDisconnected;

        private List<IMessageHandler> _handlers;
        private UdpServer _server;
        private ClientManager _clientManager;

        public IEnumerable<string> Clients => _clientManager.Clients;

        public Server(int listenPort)
        {
            // Bootstrap
            _server = new UdpServer(listenPort);
            _clientManager = new ClientManager();
            _handlers = new List<IMessageHandler>(10);

            // MessageHandlers
            _handlers.Add(new HelloHandler(this, _server, _clientManager));
            _handlers.Add(new PingHandler(this, _server, _clientManager));
            _handlers.Add(new DataHandler(this, _clientManager));

            _server.StartListening();
        }

        /// <summary>
        /// Ticks the server which processes received data.
        /// </summary>
        public void Tick()
        {
            if (_server.ReceivedMessageQueue.Count > 0)
            {
                ClientMessage messageToProcess = _server.ReceivedMessageQueue.Dequeue();

                // Parse data
                Message parsedMessage = DataParser.ParseData(messageToProcess);

                // Debug
                if (parsedMessage.Command.Equals("ping") == false)
                {
                    Console.WriteLine($"{messageToProcess.Address}:{messageToProcess.Port} says {parsedMessage.Command}:{parsedMessage.JsonData}");
                }

                // Handle message
                foreach (IMessageHandler handler in _handlers)
                {
                    if (handler.Command.Equals(parsedMessage.Command))
                    {
                        handler.Handle(parsedMessage);
                    }
                }
            }

            foreach (IMessageHandler handler in _handlers)
            {
                handler.Tick();
            }
        }

        /// <summary>
        /// Send the given message to a specific client.
        /// </summary>
        /// <param name="clientGuid">The Guid of the client you want to send data too.</param>
        /// <param name="message">The message you want to send.</param>
        public void SendMessageToClient(string clientGuid, string message)
        {
            if (_clientManager.TryGetClient(clientGuid, out Client client))
            {
                ServerMessage serverMessage = CreateDataMessege(message);
                _server.SendData(client.Address, client.Port, DataParser.SerializeData(serverMessage));
            }
        }

        private ServerMessage CreateDataMessege(string message)
        {
            return new ServerMessage()
            {
                Command = "data",
                JsonString = message,
            };
        }

        public void SendMessageToAllClients(string message)
        {
            foreach (string clientGuid in _clientManager.Clients)
            {
                SendMessageToClient(clientGuid, message);
            }
        }

        internal void TriggerMessageReceived(string clientGuid, string message)
        {
            MessageReceived?.Invoke(clientGuid, message);
        }

        internal void TriggerClientConnected(string clientGuid)
        {
            ClientConnected?.Invoke(clientGuid);
        }

        internal void TriggerClientDisconnected(string clientGuid)
        {
            ClientDisconnected?.Invoke(clientGuid);
        }
    }
}