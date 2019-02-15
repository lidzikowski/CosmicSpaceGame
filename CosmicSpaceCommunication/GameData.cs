using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace CosmicSpaceCommunication
{
    [Serializable]
    public class GameData
    {
        public static string ServerIP = "ws://localhost:24231";
        public static string Salt = "a$10${0}rBV2J";
        

        public static byte[] Serialize(object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static CommandData Deserialize(byte[] array)
        {
            if (array == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(array))
            {
                return (CommandData)bf.Deserialize(ms);
            }
        }

        public static string HashString(string password)
        {
            var data = Encoding.UTF8.GetBytes(string.Format(Salt, password));
            using (SHA512 shaM = new SHA512Managed())
            {
                return GetStringFromHash(shaM.ComputeHash(data));
            }
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
    }
}