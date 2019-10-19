using System.Security.Cryptography;
using System.Text;

namespace CosmicSpaceWebsiteDll
{
    public class Hash
    {
        public static string Salt = "a$10${0}rBV2J";

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