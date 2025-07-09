using Microsoft.IdentityModel.Tokens;
using NewsAggregation.Constants;
using NewsAggregation.Exceptions;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NewsAggregation.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ApiException(
                ErrorResponse.ErrorEnum.NullObject,
                ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));
        }

        public string GenerateToken(string userId, string username, string email, IList<string> roles)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
                throw new ApiException(ErrorResponse.ErrorEnum.Validation, ErrorMessages.InvalidModelState);

            if (roles == null || roles.Count == 0)
                throw new ApiException(ErrorResponse.ErrorEnum.Validation, ErrorMessages.InvalidModelState);

            var jwtSettings = _configuration.GetSection(AppConstants.JwtSection);
            var secretKey = jwtSettings[AppConstants.JwtSecretKey];
            var issuer = jwtSettings[AppConstants.JwtIssuer];
            var audience = jwtSettings[AppConstants.JwtAudience];

            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
                throw new ApiException(ErrorResponse.ErrorEnum.InternalServerError, ErrorMessages.UnexpectedError);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(JwtRegisteredClaimNames.Email, email),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
