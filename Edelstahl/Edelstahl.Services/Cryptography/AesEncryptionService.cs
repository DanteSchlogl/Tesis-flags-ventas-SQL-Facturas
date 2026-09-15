using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Edelstahl.Services.Cryptography
{
    /// <summary>
    /// Proporciona cifrado simétrico AES para proteger
    /// campos sensibles del sistema Edelstahl.
    /// </summary>
    public sealed class AesEncryptionService
    {
        private const int SaltSize = 16;
        private const int IvSize = 16;
        private const int KeySize = 32;
        private const int AuthenticationKeySize = 32;
        private const int AuthenticationTagSize = 32;
        private const int Iterations = 100000;

        private readonly string _secret;

        public AesEncryptionService(string secret)
        {
            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new ArgumentException(
                    "La clave de cifrado no puede estar vacía.",
                    nameof(secret));
            }

            if (secret.Length < 16)
            {
                throw new ArgumentException(
                    "La clave de cifrado debe contener " +
                    "al menos 16 caracteres.",
                    nameof(secret));
            }

            _secret = secret;
        }

        /// <summary>
        /// Cifra un texto utilizando AES.
        /// </summary>
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                return string.Empty;
            }

            byte[] salt = GenerateRandomBytes(SaltSize);
            byte[] iv = GenerateRandomBytes(IvSize);

            byte[] encryptionKey;
            byte[] authenticationKey;

            DeriveKeys(
                salt,
                out encryptionKey,
                out authenticationKey);

            byte[] plainBytes =
                Encoding.UTF8.GetBytes(plainText);

            byte[] cipherBytes;

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = encryptionKey;
                aes.IV = iv;

                using (ICryptoTransform encryptor =
                    aes.CreateEncryptor())
                {
                    cipherBytes =
                        encryptor.TransformFinalBlock(
                            plainBytes,
                            0,
                            plainBytes.Length);
                }
            }

            byte[] dataToAuthenticate =
                Combine(
                    salt,
                    iv,
                    cipherBytes);

            byte[] authenticationTag;

            using (HMACSHA256 hmac =
                new HMACSHA256(authenticationKey))
            {
                authenticationTag =
                    hmac.ComputeHash(
                        dataToAuthenticate);
            }

            byte[] encryptedPackage =
                Combine(
                    dataToAuthenticate,
                    authenticationTag);

            ClearSensitiveData(
                encryptionKey,
                authenticationKey,
                plainBytes);

            return Convert.ToBase64String(
                encryptedPackage);
        }

        /// <summary>
        /// Descifra un texto generado por Encrypt.
        /// </summary>
        public string Decrypt(string encryptedText)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
            {
                return string.Empty;
            }

            byte[] encryptedPackage;

            try
            {
                encryptedPackage =
                    Convert.FromBase64String(
                        encryptedText);
            }
            catch (FormatException ex)
            {
                throw new CryptographicException(
                    "El texto cifrado no posee un formato válido.",
                    ex);
            }

            int minimumLength =
                SaltSize +
                IvSize +
                AuthenticationTagSize +
                1;

            if (encryptedPackage.Length < minimumLength)
            {
                throw new CryptographicException(
                    "El contenido cifrado está incompleto.");
            }

            byte[] salt =
                ExtractBytes(
                    encryptedPackage,
                    0,
                    SaltSize);

            byte[] iv =
                ExtractBytes(
                    encryptedPackage,
                    SaltSize,
                    IvSize);

            int cipherLength =
                encryptedPackage.Length -
                SaltSize -
                IvSize -
                AuthenticationTagSize;

            byte[] cipherBytes =
                ExtractBytes(
                    encryptedPackage,
                    SaltSize + IvSize,
                    cipherLength);

            byte[] storedAuthenticationTag =
                ExtractBytes(
                    encryptedPackage,
                    SaltSize + IvSize + cipherLength,
                    AuthenticationTagSize);

            byte[] encryptionKey;
            byte[] authenticationKey;

            DeriveKeys(
                salt,
                out encryptionKey,
                out authenticationKey);

            byte[] dataToAuthenticate =
                Combine(
                    salt,
                    iv,
                    cipherBytes);

            byte[] calculatedAuthenticationTag;

            using (HMACSHA256 hmac =
                new HMACSHA256(authenticationKey))
            {
                calculatedAuthenticationTag =
                    hmac.ComputeHash(
                        dataToAuthenticate);
            }

            if (!AreEqualConstantTime(
                storedAuthenticationTag,
                calculatedAuthenticationTag))
            {
                throw new CryptographicException(
                    "No se pudo verificar la integridad " +
                    "del contenido cifrado.");
            }

            byte[] plainBytes;

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = encryptionKey;
                aes.IV = iv;

                using (ICryptoTransform decryptor =
                    aes.CreateDecryptor())
                {
                    plainBytes =
                        decryptor.TransformFinalBlock(
                            cipherBytes,
                            0,
                            cipherBytes.Length);
                }
            }

            string plainText =
                Encoding.UTF8.GetString(
                    plainBytes);

            ClearSensitiveData(
                encryptionKey,
                authenticationKey,
                plainBytes);

            return plainText;
        }

        private void DeriveKeys(
            byte[] salt,
            out byte[] encryptionKey,
            out byte[] authenticationKey)
        {
            using (Rfc2898DeriveBytes derivation =
                new Rfc2898DeriveBytes(
                    _secret,
                    salt,
                    Iterations,
                    HashAlgorithmName.SHA256))
            {
                encryptionKey =
                    derivation.GetBytes(KeySize);

                authenticationKey =
                    derivation.GetBytes(
                        AuthenticationKeySize);
            }
        }

        private static byte[] GenerateRandomBytes(
            int length)
        {
            byte[] bytes = new byte[length];

            using (RandomNumberGenerator generator =
                RandomNumberGenerator.Create())
            {
                generator.GetBytes(bytes);
            }

            return bytes;
        }

        private static byte[] Combine(
            params byte[][] arrays)
        {
            int totalLength = 0;

            foreach (byte[] array in arrays)
            {
                totalLength += array.Length;
            }

            byte[] result =
                new byte[totalLength];

            int destinationIndex = 0;

            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(
                    array,
                    0,
                    result,
                    destinationIndex,
                    array.Length);

                destinationIndex += array.Length;
            }

            return result;
        }

        private static byte[] ExtractBytes(
            byte[] source,
            int startIndex,
            int length)
        {
            byte[] result = new byte[length];

            Buffer.BlockCopy(
                source,
                startIndex,
                result,
                0,
                length);

            return result;
        }

        private static bool AreEqualConstantTime(
            byte[] first,
            byte[] second)
        {
            if (first == null ||
                second == null ||
                first.Length != second.Length)
            {
                return false;
            }

            int difference = 0;

            for (int i = 0; i < first.Length; i++)
            {
                difference |= first[i] ^ second[i];
            }

            return difference == 0;
        }

        private static void ClearSensitiveData(
            params byte[][] arrays)
        {
            foreach (byte[] array in arrays)
            {
                if (array != null)
                {
                    Array.Clear(
                        array,
                        0,
                        array.Length);
                }
            }
        }
    }
}