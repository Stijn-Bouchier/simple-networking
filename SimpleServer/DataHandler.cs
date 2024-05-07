namespace SimpleServer
{
    internal class DataHandler : IMessageHandler
    {
        private Server _simpleServer;
        private ClientManager _clientManager;

        public string Command => "data";

        public DataHandler(Server simpleServer, ClientManager clientManager)
        {
            _simpleServer = simpleServer;
            _clientManager = clientManager;
        }

        public void Handle(Message message)
        {
            Client sourceClient;

            if (_clientManager.TryGetClient(message.Guid, out sourceClient))
            {
                _simpleServer.TriggerMessageReceived(sourceClient.Guid, message.JsonData);
            }
        }

        public void Tick()
        {
            // Nothing to do
        }
    }
}