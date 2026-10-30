using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Identity;

namespace Bison.Models;

public sealed class User : IdentityUser
{
    public required string Name { get; set; }

    public List<Post> Posts { get; set; } = [];

    public User() : base()
    {

    }

    [SetsRequiredMembers]
    public User(string username, string email, string name) : base()
    {
        UserName = username;
        Email = email;

        Name = name;
    }
}
