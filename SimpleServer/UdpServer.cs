using System.Net.Sockets;
using System.Net;

namespace SimpleServer
{
    internal class UdpServer
    {
        private UdpClient receiveClient;

        public int Port { get; }

        public Queue<ClientMessage> ReceivedMessageQueue { get; }

        public UdpServer(int port)
        {
            Port = port;

            ReceivedMessageQueue = new Queue<ClientMessage>(100);

            IPEndPoint clientConnectionEndPoint = new IPEndPoint(IPAddress.Any, Port);
            receiveClient = new UdpClient();
            receiveClient.Client.Bind(clientConnectionEndPoint);
        }

        public void StartListening()
        {
            receiveClient.BeginReceive(HandleDataReceived, null);
        }

        private void HandleDataReceived(IAsyncResult ar)
        {
            IPEndPoint? endpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedData = receiveClient.EndReceive(ar, ref endpoint);

            if (receivedData == null || endpoint == null)
            {
                Console.WriteLine("Didn't receive any data, but still handling data reception. Early returning and stopping listening.");
                return;
            }

            ReceivedMessageQueue.Enqueue(new ClientMessage(receivedData, endpoint.Address, endpoint.Port));

            receiveClient.BeginReceive(HandleDataReceived, null);
        }

        public void Close()
        {
            receiveClient.Close();
        }

        public void SendData(IPAddress address, int port, byte[] data)
        {
            // Task.Run(() =>
            // {
            using (UdpClient sendClient = new UdpClient())
            {
                sendClient.Send(data, data.Length, new IPEndPoint(address, port));
            }
            //});
        }
    }
}