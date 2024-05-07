using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace SimpleClient
{
    public class ServerConnection
    {
        private readonly UdpClient _receiveClient;

        public string ClientGuid { get; }

        private readonly int _serverPort;
        private readonly string _serverAddress;

        public delegate void MessageReceivedDelegate(string message);

        public delegate void OtherClientConnectedDelegate(string guid);

        public delegate void OtherClientDisconnectedDelegate(string guid);

        public event MessageReceivedDelegate MessageReceived;
        public event OtherClientConnectedDelegate OtherClientConnected;
        public event OtherClientDisconnectedDelegate OtherClientDisconnected;

        private Queue<ServerMessage> _messageQueue;

        public ServerConnection(string serverAddress, int serverPort)
        {
            ClientGuid = Guid.NewGuid().ToString();
            _serverAddress = serverAddress;
            _serverPort = serverPort;

            _messageQueue = new Queue<ServerMessage>(20);

            _receiveClient = new UdpClient(0);
        }

        public void Connect()
        {
            _receiveClient.BeginReceive(HandleServerDataReceived, null);

            ClientHelloData helloData = new ClientHelloData()
            {
                Port = ((IPEndPoint)_receiveClient.Client.LocalEndPoint).Port,
                ClientGuid = ClientGuid
            };

            string jsonData = JsonUtility.ToJson(helloData);

            SendMessageToServer("hello", jsonData);
        }

        public void Tick()
        {
            while (_messageQueue.Count > 0)
            {
                ServerMessage serverMessage = _messageQueue.Dequeue();

                switch (serverMessage.Command)
                {
                    case "pong":
                        SendMessageToServer("ping", string.Empty);
                        // TODO time between ping and pong to know you're disconnected
                        break;
                    case "connect":
                        OtherClientConnected?.Invoke(serverMessage.JsonString);
                        break;
                    case "disconnect":
                        OtherClientDisconnected?.Invoke(serverMessage.JsonString);
                        break;
                    case "data":
                        MessageReceived?.Invoke(serverMessage.JsonString);
                        break;
                }
            }
        }

        private void HandleServerDataReceived(IAsyncResult ar)
        {
            IPEndPoint serverConnectionEndpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedData = _receiveClient.EndReceive(ar, ref serverConnectionEndpoint);

            ServerMessage serverMessage = DataParser.ParseData(receivedData);
            
            _messageQueue.Enqueue(serverMessage);

            _receiveClient.BeginReceive(HandleServerDataReceived, null);
        }

        public void SendMessageToServer(string message)
        {
            SendMessageToServer("data", message);
        }

        private void SendMessageToServer(string command, string message)
        {
            using (UdpClient sendClient = new UdpClient())
            {
                Message sendMessage = new Message()
                {
                    Guid = ClientGuid,
                    Command = command,
                    JsonData = message
                };

                byte[] data = DataParser.SerializeData(sendMessage);
                sendClient.Send(data, data.Length, _serverAddress, _serverPort);
            }
        }
    }
}