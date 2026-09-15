namespace Edelstahl.Services.Cryptography
{
    /// <summary>
    /// Proporciona una fachada para las operaciones
    /// criptográficas utilizadas por Edelstahl.
    /// </summary>
    public static class CryptographyService
    {
        /// <summary>
        /// Cifra un texto mediante el servicio AES
        /// configurado para la aplicación.
        /// </summary>
        public static string Encrypt(
            string plainText)
        {
            AesEncryptionService encryptionService =
                CreateEncryptionService();

            return encryptionService.Encrypt(
                plainText);
        }

        /// <summary>
        /// Descifra un texto previamente generado
        /// mediante el servicio AES.
        /// </summary>
        public static string Decrypt(
            string encryptedText)
        {
            AesEncryptionService encryptionService =
                CreateEncryptionService();

            return encryptionService.Decrypt(
                encryptedText);
        }

        private static AesEncryptionService
            CreateEncryptionService()
        {
            string secret =
                CryptographySettings
                    .GetEncryptionSecret();

            return new AesEncryptionService(
                secret);
        }
    }
}
