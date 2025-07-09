using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Controllers;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Exceptions;
using NewsAggregation.Utilities;

namespace UnitTests.Controllers
{
    [TestClass]
    public class AuthControllerTests
    {
        #region Private Fields

        private Mock<IAuthService> _authServiceMock;
        private Mock<ILogger<AuthController>> _loggerMock;
        private AuthController _controller;

        #endregion

        #region Constructor

        [TestInitialize]
        public void Setup()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_authServiceMock.Object, _loggerMock.Object);
        }

        #endregion

        [TestMethod]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            var loginDto = new LoginDto { Username = "user", Password = "pass" };
            var userDataWithToken = new UserDataWithTokenDto
            {
                User = new UserReadDto { Username = "user", Email = "a@b.com" },
                token = "token"
            };
            _authServiceMock.Setup(x => x.LoginAsync(loginDto)).ReturnsAsync(userDataWithToken);

            var result = await _controller.Login(loginDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(userDataWithToken, okResult.Value);
        }

        [TestMethod]
        public async Task Login_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Username", "Required");
            var loginDto = new LoginDto();

            var result = await _controller.Login(loginDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual((int)ErrorResponse.ErrorEnum.Validation, objectResult.StatusCode);
        }

        [TestMethod]
        public async Task Login_ShouldReturnError_WhenAuthServiceThrowsApiException()
        {
            var loginDto = new LoginDto { Username = "user", Password = "pass" };
            _authServiceMock.Setup(x => x.LoginAsync(loginDto)).ThrowsAsync(new ApiException(ErrorResponse.ErrorEnum.InternalServerError, "Custom error"));

            var result = await _controller.Login(loginDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
        }
    }
}
