using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace GameCollectionApi.Services
{
    public static class ApiKeyHelper
    {
        public static string Generate()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);
            return WebEncoders.Base64UrlEncode(bytes);
        }

        public static string Hash(string apiKey)
        {
            byte[] bytes = SHA256.HashData(
                Encoding.UTF8.GetBytes(apiKey));

            return Convert.ToHexString(bytes);
        }
    }
}
