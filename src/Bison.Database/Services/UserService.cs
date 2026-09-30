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
    public Task<User> CreateUserAsync(string name)
    {
        return _repository.CreateUser(name);
    }
}
