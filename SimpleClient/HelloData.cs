using System;

namespace SimpleClient
{
    [Serializable]
    internal struct ClientHelloData
    {
        public int Port;
        public string ClientGuid;
    }
}