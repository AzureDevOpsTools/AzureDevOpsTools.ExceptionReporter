using System;
using System.Text;
using System.Security.Cryptography;

namespace AzureDevOps.Exception.Service.Common.Sec
{
    public class Encryption
    {
        private readonly string Key = "AzureDevOps.Exception.Reporter.Key";
        private readonly byte[] IVector = new byte[8] { 27, 9, 45, 27, 0, 72, 171, 54 };

        public string Encrypt(string inputString)
        {
            if (String.IsNullOrEmpty(inputString))
                return String.Empty;

            byte[] buffer = Encoding.ASCII.GetBytes(inputString);
            var tripleDes = new TripleDESCryptoServiceProvider();
            var md5 = new MD5CryptoServiceProvider();
            tripleDes.Key = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(Key));
            tripleDes.IV = IVector;
            var ITransform = tripleDes.CreateEncryptor();
            return Convert.ToBase64String(ITransform.TransformFinalBlock(buffer, 0, buffer.Length));
        }

        public string Decrypt(string inputString)
        {
            if (String.IsNullOrEmpty(inputString))
                return String.Empty;

            byte[] buffer = Convert.FromBase64String(inputString);
            var tripleDes = new TripleDESCryptoServiceProvider();
            var md5 = new MD5CryptoServiceProvider();
            tripleDes.Key = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(Key));
            tripleDes.IV = IVector;
            var ITransform = tripleDes.CreateDecryptor();
            return Encoding.ASCII.GetString(ITransform.TransformFinalBlock(buffer, 0, buffer.Length));
        }
    }
}