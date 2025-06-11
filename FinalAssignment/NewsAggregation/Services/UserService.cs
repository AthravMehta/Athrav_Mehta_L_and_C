using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services;
using System.Text.RegularExpressions;

public class UserService : CrudBaseService<User, Guid>, IUserService
{
    private readonly IMapper _mapper;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly IUserRepository _userRepository;

    public UserService(ICrudBaseRepository<User, Guid> repository, IMapper mapper, IUserRepository userRepository) : base(repository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<UserReadDto> CreateUserAsync(UserCreateDto userDto)
    {
        ValidateUserCreateDto(userDto);
        var user = _mapper.Map<User>(userDto);

        user.Id = Guid.NewGuid();
        user.PasswordHash = HashPassword(user, userDto.Password);
        user.CreatedDateTime = DateTime.UtcNow;
        user.LastUpdatedDateTime = DateTime.UtcNow;

        await AddAsync(user);

        return _mapper.Map<UserReadDto>(user);
    }

    public async Task<UserReadDto> UpdateUserAsync(Guid id, UserUpdateDto userDto)
    {
        var existingUser = await GetByIdAsync(id);
        if (existingUser == null)
            throw new ApiException("User not found");

        ValidateUserUpdateDto(userDto);

        if (!string.IsNullOrWhiteSpace(userDto.Username))
            existingUser.Username = userDto.Username;

        if (!string.IsNullOrWhiteSpace(userDto.Password))
            existingUser.PasswordHash = HashPassword(existingUser, userDto.Password);

        if (!string.IsNullOrWhiteSpace(userDto.Email))
            existingUser.Email = userDto.Email;

        if (userDto.RoleId.HasValue)
            existingUser.RoleId = userDto.RoleId.Value;

        existingUser.LastUpdatedDateTime = DateTime.UtcNow;

        await UpdateAsync(existingUser);

        return _mapper.Map<UserReadDto>(existingUser);
    }

    public async Task<UserReadDto> GetUserByIdAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user == null)
            return null;

        return _mapper.Map<UserReadDto>(user);
    }

    public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync()
    {
        var users = await GetAllAsync();
        return _mapper.Map<IEnumerable<UserReadDto>>(users);
    }

    public async Task<User> GetUserByName(string username)
    {
        var user = await _userRepository.GetUserByName(username);
        return user;
    }

    public bool VerifyPassword(User user, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, providedPassword);
        return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
    }

    private string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    private void ValidateUserCreateDto(UserCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length < 8)
            throw new ApiException("Username must be at least 8 characters long.");

        if (string.IsNullOrWhiteSpace(dto.Password) || !IsValidPassword(dto.Password))
            throw new ApiException("Password must be at least 8 characters long and contain uppercase, lowercase, and special character.");

        if (string.IsNullOrWhiteSpace(dto.Email) || !IsValidEmail(dto.Email))
            throw new ApiException("Email format is invalid.");
    }

    private void ValidateUserUpdateDto(UserUpdateDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username.Length < 8)
            throw new ApiException("Username must be at least 8 characters long.");

        if (!string.IsNullOrWhiteSpace(dto.Password) && !IsValidPassword(dto.Password))
            throw new ApiException("Password must be at least 8 characters long and contain uppercase, lowercase, and special character.");

        if (!string.IsNullOrWhiteSpace(dto.Email) && !IsValidEmail(dto.Email))
            throw new ApiException("Email format is invalid.");
    }

    private bool IsValidPassword(string password)
    {
        // At least 8 chars, 1 uppercase, 1 lowercase, 1 special char
        var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$");
        return regex.IsMatch(password);
    }

    // Email regex validation
    private bool IsValidEmail(string email)
    {
        // Simple email regex pattern (@)(.)
        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return regex.IsMatch(email);
    }

}
