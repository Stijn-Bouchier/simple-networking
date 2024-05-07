namespace SimpleServer
{
    [Serializable]
    internal struct ServerMessage
    {
        public string Command { get; set; }
        public string JsonString { get; set; }
    }
}