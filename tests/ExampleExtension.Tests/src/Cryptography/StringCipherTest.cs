using System;
using System.Security.Cryptography;
using Xunit;
using ExampleExtension.Cryptography;

namespace ExampleExtension.Tests.Cryptography
{
    public class StringCipherTest
    {
        [Fact]
        public void EncryptAndDecrypt()
        {
            // ARRANGE

            string passPhrase = GeneratePassphrase();
            string plainText = "Some text to keep secret!";

            // ACT

            string cipherText = StringCipher.Encrypt(plainText, passPhrase);
            string decryptedText = StringCipher.Decrypt(cipherText, passPhrase);

            // ASSERT

            Assert.Contains("??", cipherText);
            Assert.Equal(plainText, decryptedText);
        }

        private string GeneratePassphrase()
        {
            byte[] randomBytes = new byte[32];
            RandomNumberGenerator.Create().GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}