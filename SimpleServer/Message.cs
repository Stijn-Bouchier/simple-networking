using System.Net;

namespace SimpleServer
{
    [Serializable]
    internal struct Message
    {
        public string Guid { get; set; }
        public string Command { get; set; }
        public string JsonData { get; set; }
        public IPAddress ClientAddress { get; set; }
        public int ClientPort { get; set; }
    }
}