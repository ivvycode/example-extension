using System;
using System.IO;
using System.Security.Cryptography;

namespace ExampleExtension.Cryptography
{
    /// <summary>
    /// This class can be used to encrypt/decrypt data.
    /// </summary>
    public sealed class StringCipher
    {
        private const string SEPARATOR = "??";

        public static string Encrypt(string plainText, string passPhrase)
        {
            byte[] key = Convert.FromBase64String(passPhrase);
            byte[] saltBytes = new byte[32];
            RandomNumberGenerator.Create().GetBytes(saltBytes);
            string salt = Convert.ToBase64String(saltBytes);
            byte[] encrypted;
            byte[] IV;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                IV = aes.IV;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(salt + plainText);
                        }
                        encrypted = memoryStream.ToArray();
                    }
                }
            }
            return string.Format(
                "{0}{1}{2}{3}{4}",
                Convert.ToBase64String(encrypted),
                SEPARATOR,
                Convert.ToBase64String(IV),
                SEPARATOR,
                salt
            );
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            string[] cipherParts = cipherText.Split(new[] { SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
            if (cipherParts.Length != 3)
            {
                throw new ArgumentException("cipherText is not a valid encrypted string");
            }
            byte[] encrypted = Convert.FromBase64String(cipherParts[0]);
            byte[] key = Convert.FromBase64String(passPhrase);
            byte[] IV = Convert.FromBase64String(cipherParts[1]);
            string salt = cipherParts[2];
            string decrypted = "";
            string plainText = "";
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = IV;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(encrypted))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            decrypted = reader.ReadToEnd();
                        }
                    }
                }
            }
            plainText = decrypted.Substring(salt.Length);
            return plainText;
        }
    }
}