using System;

namespace SimpleServer
{
    [Serializable]
    internal struct ClientHelloData
    {
        public int Port { get; set; }
        public string ClientGuid { get; set; }
    }
}