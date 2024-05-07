using System.Text;
using UnityEngine;

namespace SimpleClient
{
    internal static class DataParser
    {
        public static ServerMessage ParseData(byte[] data)
        {
            string dataString = Encoding.UTF8.GetString(data);

            return JsonUtility.FromJson<ServerMessage>(dataString);
        }

        public static byte[] SerializeData(Message data)
        {
            return Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
        }
    }
}