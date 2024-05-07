using System.Net;

namespace SimpleServer
{
    internal struct ClientMessage
    {
        public byte[] Data { get; }
        public IPAddress Address { get; }
        public int Port { get; }

        public ClientMessage(byte[] data, IPAddress address, int port)
        {
            Data = data;
            Address = address;
            Port = port;
        }
    }
}