using Microsoft.AspNetCore.DataProtection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NewsAggregation.Exceptions;
using NewsAggregation.Services;
using NewsAggregation.Services.Contracts;
using System;

namespace UnitTests.Services
{
    [TestClass]
    public class EncryptionServiceTests
    {
        #region Private Fields

        private Mock<IDataProtectionProvider> _dataProtectionProviderMock;
        private Mock<IDataProtector> _dataProtectorMock;
        private EncryptionService _service;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _dataProtectionProviderMock = new Mock<IDataProtectionProvider>();
            _dataProtectorMock = new Mock<IDataProtector>();
            _dataProtectionProviderMock.Setup(x => x.CreateProtector(It.IsAny<string>())).Returns(_dataProtectorMock.Object);
            _service = new EncryptionService(_dataProtectionProviderMock.Object);
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ShouldCreateService_WhenValidProvider()
        {
            var service = new EncryptionService(_dataProtectionProviderMock.Object);
            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void Constructor_ShouldThrowApiException_WhenProviderIsNull()
        {
            Assert.ThrowsException<ApiException>(() => new EncryptionService(null));
        }

        #endregion

        #region Encrypt Tests

        [TestMethod]
        public void Encrypt_ShouldThrowApiException_WhenPlainTextIsNull()
        {
            Assert.ThrowsException<ApiException>(() => _service.Encrypt(null));
        }

        #endregion

        #region Decrypt Tests


        [TestMethod]
        public void Decrypt_ShouldThrowApiException_WhenCipherTextIsNull()
        {
            Assert.ThrowsException<ApiException>(() => _service.Decrypt(null));
        }

        [TestMethod]
        public void Verify_ShouldThrowApiException_WhenEncryptedValueIsNull()
        {
            Assert.ThrowsException<ApiException>(() => _service.Verify(null, "test"));
        }

        [TestMethod]
        public void Verify_ShouldThrowApiException_WhenPlainTextToCompareIsNull()
        {
            Assert.ThrowsException<ApiException>(() => _service.Verify("encrypted", null));
        }

        [TestMethod]
        public void Verify_ShouldThrowApiException_WhenBothParametersAreNull()
        {
            Assert.ThrowsException<ApiException>(() => _service.Verify(null, null));
        }

        #endregion
    }
} 