using NewsAggregationConsole.Models;

namespace NewsAggregationConsole.Services
{
    public class AuthService
    {
        private readonly ApiService _apiService;

        public AuthService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<UserDto> LoginAsync(LoginDto loginDto)
        {
            var userDataWithToken = await _apiService.PostAsync<UserDataWithTokenDto>(
                "/api/auth/login", loginDto);

            _apiService.SetAuthToken(userDataWithToken.token);
            return userDataWithToken.User;
        }
        
        public async Task<UserDto> SignUpAsync(RegisterDto registerDto)
        {
            var userDataWithToken = await _apiService.PostAsync<UserDataWithTokenDto>(
                "/api/user", registerDto);

            _apiService.SetAuthToken(userDataWithToken.token);
            return userDataWithToken.User;
        }

        public void LogoutAsync()
        {
            _apiService.SetAuthToken(String.Empty);
        }
    }
}
