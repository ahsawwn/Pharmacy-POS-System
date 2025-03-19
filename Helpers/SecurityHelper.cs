using System.Security.Cryptography;
using System.Text;

namespace POS.Helpers
{
    public static class SecurityHelper
    {
        public static string ComputeSHA256Hex(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder hexString = new StringBuilder();
                foreach (byte b in bytes)
                {
                    hexString.Append(b.ToString("x2")); // Convert bytes to hexadecimal string
                }
                return hexString.ToString();
            }
        }
    }
}
