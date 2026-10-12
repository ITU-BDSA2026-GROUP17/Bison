namespace Bison.Database.Services;

public class UserService(IDatabaseRepository databaseRepository) : IUserService
{
    private readonly IDatabaseRepository _repository = databaseRepository;

    public async Task<UserDTO?> GetUserByIdAsync(int userId)
    {
        var user = await _repository.GetUserById(userId);
        return user is null ? null : new UserDTO(user.Id, user.Name, user.Email);
    }
    public async Task<UserDTO?> GetUserByNameAsync(string name)
    {
        var user = await _repository.GetUserByName(name);
        return user is null ? null : new UserDTO(user.Id, user.Name, user.Email);
    }
    public async Task<UserDTO> CreateUserAsync(string name, string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (!IUserService.IsValidEmail(email))
            throw new ArgumentException("email is invalid", nameof(email));

        var user = await _repository.CreateUser(name, email);
        return new UserDTO(user.Id, user.Name, user.Email);
    }
}
