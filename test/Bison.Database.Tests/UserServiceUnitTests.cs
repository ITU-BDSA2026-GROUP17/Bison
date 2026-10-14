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
    public static async Task CreateUserDTOTest(NonWhiteSpaceString name, MailAddress email)
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        // Act
        var userDTO = await db.CreateUserAsync(name.Item, email.Address);
        var nameUserDTO = await db.GetUserByNameAsync(name.Item);
        var idUserDTO = await db.GetUserByIdAsync(userDTO.Id);

        // Assert
        userDTO.Should().NotBeNull();
        userDTO.Name.Should().Be(name.Item);

        nameUserDTO.Id.Should().Be(userDTO.Id);
        nameUserDTO.Name.Should().Be(userDTO.Name);

        idUserDTO.Id.Should().Be(userDTO.Id);
        idUserDTO.Name.Should().Be(userDTO.Name);
    }

    [Property]
    public async Task MapUserDTOToUserTest(int userId)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        var userDTO = await db.GetUserByIdAsync(userId);
        var user = await db.ToUser(userDTO);

        //Assert
        userDTO.Name.Should().Be(user.Name);
        userDTO.Email.Should().Be(user.Email);
        userDTO.Id.Should().Be(user.Id);
    }
}
