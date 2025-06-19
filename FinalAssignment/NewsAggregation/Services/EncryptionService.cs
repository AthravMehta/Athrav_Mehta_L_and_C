using Microsoft.AspNetCore.DataProtection;
using NewsAggregation.Services.Contracts;

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
            if (plainText == null) throw new ArgumentNullException(nameof(plainText));
            return _protector.Protect(plainText);
        }

        public string Decrypt(string cipherText)
        {
            if (cipherText == null) throw new ArgumentNullException(nameof(cipherText));
            return _protector.Unprotect(cipherText);
        }

        public bool Verify(string encryptedValue, string plainTextToCompare)
        {
            var decrypted = Decrypt(encryptedValue);
            return decrypted == plainTextToCompare ? true : false;
        }
    }
}
