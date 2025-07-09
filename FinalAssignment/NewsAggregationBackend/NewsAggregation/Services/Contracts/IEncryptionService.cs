namespace NewsAggregation.Services.Contracts
{
    public interface IEncryptionService
    {
        /// <summary>
        /// To Encrypt any text
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        string Encrypt(string plainText);

        /// <summary>
        /// To Decrypt any text
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        string Decrypt(string cipherText);

        /// <summary>
        /// Check If the Encrypted values matches the entered value or not.
        /// </summary>
        /// <param name="encryptedValue"></param>
        /// <param name="plainTextToCompare"></param>
        /// <returns></returns>
        bool Verify(string encryptedValue, string plainTextToCompare);
    }
}
