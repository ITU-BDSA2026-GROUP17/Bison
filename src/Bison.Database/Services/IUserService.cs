namespace Bison.Database.Services;

using System.Net.Mail;

using Bison.Models;

public interface IUserService
{
    public Task<User?> GetUserByIdAsync(int userId);
    public Task<User?> GetUserByNameAsync(string name);
    public Task<User> CreateUserAsync(string name, string email);

    public static bool IsValidEmail(string? email) {
        if (email is null) return false;
        var trimmed = email.Trim();
        try {
            var addr = new MailAddress(email);
            return addr.Address == trimmed;
        } catch {
            return false;
        }
    }
}
