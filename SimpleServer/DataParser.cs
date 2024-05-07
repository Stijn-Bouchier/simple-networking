using System.Text;
using System.Text.Json;

namespace SimpleServer
{
    internal static class DataParser
    {
        public static Message ParseData(ClientMessage message)
        {
            string dataString = Encoding.UTF8.GetString(message.Data);

            Message parsedMessage = JsonSerializer.Deserialize<Message>(dataString);

            parsedMessage.ClientAddress = message.Address;
            parsedMessage.ClientPort = message.Port;

            return parsedMessage;
        }

        public static byte[] SerializeData(ServerMessage data)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
        }
    }
}
