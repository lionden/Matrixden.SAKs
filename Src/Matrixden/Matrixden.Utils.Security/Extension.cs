using Matrixden.Utils.Extensions;
using Matrixden.Utils.Security.Logging;
using System;

namespace Matrixden.Utils.Security
{
    public static class Extension
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        public static string RSAValue(this string tobeEncrypt, string publicKey)
        {
            if (publicKey.IsNullOrEmptyOrWhiteSpace())
            {
                throw new ArgumentNullException("Public key MUST NOT be null or empty, please check it.");
            }

            RSACryptoService rsa = new RSACryptoService(null, publicKey);
            return rsa.Encrypt(tobeEncrypt);
        }

        public static bool TryDecrypt_RSA(string cipherText, string privateKey, out string text)
        {
            text = string.Empty;
            if (privateKey.IsNullOrEmptyOrWhiteSpace())
                return false;

            RSACryptoService rsa = new RSACryptoService(privateKey);
            try
            {
                text = rsa.Decrypt(cipherText);
                return true;
            }
            catch (Exception ex)
            {
                log.ErrorException("Failed to decrypt cipher text[{0}].", ex, cipherText);
                return false;
            }
        }

        public static string Decrypt_RSA(this string cipherText, string privateKey)
        {
            string text;
            if (!TryDecrypt_RSA(cipherText, privateKey, out text))
            {
                log.WarnFormat("Decrypt fail. Cipher Text: '{0}'.", cipherText);
            }

            return text;
        }
    }
}