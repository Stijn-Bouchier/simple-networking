namespace SimpleServer
{
    internal interface IMessageHandler
    {
        string Command { get; }

        void Handle(Message message);
        void Tick();
    }
}