using Bison.Models;

namespace Bison.Database.Services;

public class UserService(IDatabaseRepository databaseRepository) : IUserService
{
    private readonly IDatabaseRepository _repository = databaseRepository;

    public Task<User?> GetUserByIdAsync(int userId)
    {
        return _repository.GetUserById(userId);
    }
    public Task<User?> GetUserByNameAsync(string name)
    {
        return _repository.GetUserByName(name);
    }
    public Task<User> CreateUserAsync(string name, string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        return !IUserService.IsValidEmail(email)
            ? throw new ArgumentException("email is invalid", nameof(email))
            : _repository.CreateUser(name, email);
    }
}
