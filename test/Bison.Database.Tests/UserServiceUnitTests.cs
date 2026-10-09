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
    public static async Task EmptyDbGetUserByNameReturnTest(string name)
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        // Act
        var user = await db.GetUserByNameAsync(name);

        // Assert
        user.Should().BeNull();
    }

    [Property]
    public static async Task CreateUserTest(string name)
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        // Act
        var user = await db.CreateUserAsync(name);
        var nameUser = await db.GetUserByNameAsync(name);
        var idUser = await db.GetUserByIdAsync(user.Id);


        // Assert
        user.Should().NotBeNull();
        user.Name.Should().Be(name);
        user.Observations.Count.Should().Be(0);

        nameUser.Id.Should().Be(user.Id);
        nameUser.Name.Should().Be(user.Name);

        idUser.Id.Should().Be(user.Id);
        idUser.Name.Should().Be(user.Name);
    }
}
