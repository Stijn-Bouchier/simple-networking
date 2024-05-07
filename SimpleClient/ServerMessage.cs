using System;

namespace SimpleClient
{
    [Serializable]
    internal struct ServerMessage
    {
        public string Command;
        public string JsonString;
    }
}