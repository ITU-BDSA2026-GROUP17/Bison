using Bison.Models;

namespace Bison.Database.Services;

public class UserService(IDatabaseRepository databaseRepository) : IUserService
{
    private readonly IDatabaseRepository _repository = databaseRepository;

    public async Task<UserDTO?> GetUserByIdAsync(string userId)
    {
        var user = await _repository.GetUserById(userId);
        return user is null ? null : IUserService.ToUserDTO(user);
    }
    public async Task<UserDTO?> GetUserByUserNameAsync(string username)
    {
        var user = await _repository.GetUserByUserName(username);
        return user is null ? null : IUserService.ToUserDTO(user);
    }
    public async Task<UserDTO> CreateUserAsync(string username, string email, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (!IUserService.IsValidEmail(email))
            throw new ArgumentException("email is invalid", nameof(email));

        var user = await _repository.CreateUser(username, email, name);
        return IUserService.ToUserDTO(user);
    }

    public async Task<User?> ToUserAsync(UserDTO userDTO)
    {
        return await _repository.GetUserByUserName(userDTO.Name);
    }
}
