namespace Bison.Database.Services;

using System.Net.Mail;

using Bison.Models;

public interface IUserService
{
    public Task<UserDTO?> GetUserByIdAsync(int userId);
    public Task<UserDTO?> GetUserByNameAsync(string name);
    public Task<UserDTO> CreateUserAsync(string name, string email);

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
        return new UserDTO(user.Id, user.Name, user.Email);
    }

    public Task<User?> ToUserAsync(UserDTO userDTO);
}

public record UserDTO(int Id, string Name, string Email);
