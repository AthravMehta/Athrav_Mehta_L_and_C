using Microsoft.AspNetCore.DataProtection;
using NewsAggregation.Constants;
using NewsAggregation.Exceptions;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;

namespace NewsAggregation.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IDataProtector _protector;

        public EncryptionService(IDataProtectionProvider provider)
        {
            if (provider == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));
            _protector = provider.CreateProtector(AppConstants.KeyProtector);
        }

        public string Encrypt(string plainText)
        {
            if (plainText == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));
            return _protector.Protect(plainText);
        }

        public string Decrypt(string cipherText)
        {
            if (cipherText == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));
            return _protector.Unprotect(cipherText);
        }

        public bool Verify(string encryptedValue, string plainTextToCompare)
        {
            if (encryptedValue == null || plainTextToCompare == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            var encrypted = Encrypt(plainTextToCompare);
            return encrypted == encryptedValue;
        }
    }
}
