using System.Net;

namespace SimpleServer
{
    internal class ClientManager
    {
        private readonly List<Client> _clients;

        public List<string> Clients { get; }

        public ClientManager()
        {
            _clients = new List<Client>(10);
            Clients = new List<string>(10);
        }

        public void AddClient(Client client)
        {
            _clients.Add(client);
            Clients.Add(client.Guid);
        }

        public bool TryGetClient(string clientGuid, out Client outClient)
        {
            int clientIndex = Clients.IndexOf(clientGuid);

            if (clientIndex > -1)
            {
                outClient = _clients[clientIndex];
                return true;
            }

            outClient = default;
            return false;
        }

        public void RemoveClient(string clientGuid)
        {
            Client clientToRemove;

            if (TryGetClient(clientGuid, out clientToRemove))
            {
                _clients.Remove(clientToRemove);
                Clients.Remove(clientToRemove.Guid);
            }
        }
    }
}