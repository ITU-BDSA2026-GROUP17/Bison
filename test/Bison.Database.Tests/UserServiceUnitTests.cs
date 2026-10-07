namespace Bison.Database.Tests;

public class UserServiceUnitTests
{
    [Property]
    public static async Task EmptyDbGetUserByIdReturnTest(int userId)
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = new DatabaseRepository(dbOptions.BisonDbContext);

        // Act
        var user = await db.GetUserById(userId);

        // Assert
        user.Should().BeNull();
    }

    [Property]
    public static async Task EmptyDbGetUserByNameReturnTest(string name)
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = new DatabaseRepository(dbOptions.BisonDbContext);

        // Act
        var user = await db.GetUserByName(name);

        // Assert
        user.Should().BeNull();
    }

    [Property]
    public static async Task CreateUserTest(string name)
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = new DatabaseRepository(dbOptions.BisonDbContext);

        // Act
        var user = await db.CreateUser(name);
        var nameUser = await db.GetUserByName(name);
        var idUser = await db.GetUserById(user.Id);


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
