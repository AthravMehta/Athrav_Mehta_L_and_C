using Microsoft.AspNetCore.DataProtection;

namespace NewsAggregation.Services
{

    public class EncryptionService
    {
        private readonly IDataProtector _protector;

        public EncryptionService(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("KeyProtector");
        }

        public string Encrypt(string plainText)
        {
            try
            {
                if (plainText == null) throw new ArgumentNullException(nameof(plainText));
                return _protector.Protect(plainText);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return "";
            }
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                if (cipherText == null) throw new ArgumentNullException(nameof(cipherText));
                return _protector.Unprotect(cipherText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "";
            }
        }

        public bool Verify(string encryptedValue, string plainTextToCompare)
        {
            var decrypted = Decrypt(encryptedValue);
            return decrypted == plainTextToCompare ? true : false;
        }
    }
}
