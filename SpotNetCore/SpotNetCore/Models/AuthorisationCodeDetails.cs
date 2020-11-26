using System;
using System.Linq;
using System.Security.Cryptography;

namespace SpotNetCore.Models
{
    public class AuthorisationCodeDetails
    {
        public AuthorisationCodeDetails()
        {
            var verifier = CreateCodeVerifier();
            CodeVerifier = verifier;
            CodeChallenge = CreateCodeChallenge(verifier);
        }
        
        public string CodeVerifier { get; }
        public string CodeChallenge { get; }
        public string AuthorisationUri { get; set; }
        public string RedirectUri { get; set; }

        private static string CreateCodeVerifier()
        {
            var charList = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.-~";

            return Enumerable.Range(0, new Random().Next(44, 127)).Aggregate("", (current, index) => current + charList[new Random().Next(0, charList.Length)]);
        }

        private static string CreateCodeChallenge(string verifier)
        {
            using (var sha = SHA256.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(verifier)));
            }
        }
    }
}