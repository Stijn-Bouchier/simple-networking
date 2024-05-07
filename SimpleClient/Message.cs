using System;

namespace SimpleClient
{
    [Serializable]
    internal struct Message
    {
        public string Guid;
        public string Command;
        public string JsonData;
    }
}