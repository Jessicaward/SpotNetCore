using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SpotNetCore.Models
{
    public class AuthorisationCodeDetails
    {
        public AuthorisationCodeDetails(string verifier, string redirectUri)
        {
            RedirectUri = redirectUri;
            CodeVerifier = verifier;
            CodeChallenge = CreateCodeChallenge(verifier);
        }
        
        public string CodeVerifier { get; set; }
        public string CodeChallenge { get; }
        public string AuthorisationUri { get; set; }
        public string RedirectUri { get; set; }

        public static string CreateCodeVerifier()
        {
            var charList = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.-~";

            return Enumerable.Range(0, new Random().Next(44, 127)).Aggregate("", (current, index) => current + charList[new Random().Next(0, charList.Length)]);
        }

        private static string CreateCodeChallenge(string verifier)
        {
            using (var sha = SHA256.Create())
            {
                return UrlEncode(sha.ComputeHash(Encoding.UTF8.GetBytes(verifier)));
            }
        }

        private static string UrlEncode(byte[] input)
        {
            var buf = new char[checked(checked(input.Length + 2) / 3 * 4)];
            var numBase64Chars = Convert.ToBase64CharArray(input, 0, input.Length, buf, 0);

            foreach (var index in Enumerable.Range(0, numBase64Chars))
            {
                switch (buf[index])
                {
                    case '+':
                        buf[index] = '-';
                        break;
                    case '/':
                        buf[index] = '_';
                        break;
                    case '=':
                        return new string(buf, startIndex: 0, length: index);
                }
            }

            return new string(buf, startIndex: 0, length: numBase64Chars);
        }
    }
}