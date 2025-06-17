namespace NewsAggregation.Services.Contracts
{
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generates a JWT token string for the specified user information and roles.
        /// </summary>
        /// <param name="userId">Unique user identifier (sub claim)</param>
        /// <param name="username">User's username</param>
        /// <param name="email">User's email</param>
        /// <param name="roles">List of user roles</param>
        /// <returns>JWT token string</returns>
        string GenerateToken(string userId, string username, string email, IList<string> roles);
    }
}
