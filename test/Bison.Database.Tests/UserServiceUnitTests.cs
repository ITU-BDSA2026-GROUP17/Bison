using System.Net.Mail;

using Bison.Database.Services;

namespace Bison.Database.Tests;

public class UserServiceUnitTests
{
    [Property]
    public static async Task EmptyDbGetUserByIdReturnTest(int userId)
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        // Act
        var user = await db.GetUserByIdAsync(userId);

        // Assert
        user.Should().BeNull();
    }

    [Property]
    public static async Task EmptyDbGetUserByNameReturnTest(NonWhiteSpaceString name)
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        // Act
        var user = await db.GetUserByNameAsync(name.Item);

        // Assert
        user.Should().BeNull();
    }

    [Property]
    public static async Task CreateUserTest(NonWhiteSpaceString name, MailAddress email)
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        // Act
        var user = await db.CreateUserAsync(name.Item, email.Address);
        var nameUser = await db.GetUserByNameAsync(name.Item);
        var idUser = await db.GetUserByIdAsync(user.Id);


        // Assert
        user.Should().NotBeNull();
        user.Name.Should().Be(name.Item);
        user.Posts.Count.Should().Be(0);

        nameUser.Id.Should().Be(user.Id);
        nameUser.Name.Should().Be(user.Name);

        idUser.Id.Should().Be(user.Id);
        idUser.Name.Should().Be(user.Name);
    }
}
