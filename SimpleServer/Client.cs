using System.Net;

namespace SimpleServer
{
    internal struct Client
    {
        public IPAddress Address { get; }
        public int Port { get; }
        internal string Guid { get; }

        internal Client(IPAddress address, int port, string guid)
        {
            Address = address;
            Port = port;
            Guid = guid;
        }
    }
}