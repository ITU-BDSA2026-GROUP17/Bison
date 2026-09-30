namespace Bison.Database.Services;

using Bison.Models;

public interface IUserService
{
    public Task<User?> GetUserByIdAsync(int userId);
    public Task<User?> GetUserByNameAsync(string name);
    public Task<User> CreateUserAsync(string name);
}
