namespace Bison.Database.Services;

using System.Net.Mail;

using Bison.Models;

public interface IUserService
{
    public Task<UserDTO?> GetUserByIdAsync(string userId);
    public Task<UserDTO?> GetUserByUserNameAsync(string username);
    public Task<UserDTO> CreateUserAsync(string username, string email, string name);

    public static bool IsValidEmail(string? email)
    {
        if (email is null) return false;
        var trimmed = email.Trim();
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == trimmed;
        }
        catch
        {
            return false;
        }
    }
    public static UserDTO ToUserDTO(User user)
    {
        return new UserDTO(user.Id, user.Name, user.UserName);
    }

    public Task<User?> ToUserAsync(UserDTO userDTO);
}

public record UserDTO(string Id, string Name, string? Username);
