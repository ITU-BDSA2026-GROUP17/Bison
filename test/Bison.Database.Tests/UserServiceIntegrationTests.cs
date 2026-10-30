using System.Net.Mail;

using Bison.Database.Services;
using Bison.Models;

using Microsoft.EntityFrameworkCore;

namespace Bison.Database.Tests;

public class UserServiceIntegrationTests
{
    public static async Task<Arbitrary<User>> UserGenerator(BisonDbContext dbContext) =>
        Gen.Elements((IEnumerable<User>)await dbContext.Users.ToListAsync()).ToArbitrary();

    [Fact]
    public static async Task DbGetUserDTOByIdTest()
    {
        // Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        var user = (await UserGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];

        // Act
        var userDTO = await db.GetUserByIdAsync(user.Id);

        // Assert
        userDTO.Should().NotBeNull();
        userDTO.Id.Should().Be(user.Id);
        userDTO.Name.Should().Be(user.Name);
    }

    [Fact]
    public static async Task DbGetUserDTOByNameTest()
    {
        // Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        var user = (await UserGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];

        // Act
        var userDTO = await db.GetUserByUserNameAsync(user.Name);

        // Assert
        userDTO.Should().NotBeNull();
        userDTO.Id.Should().Be(user.Id);
        userDTO.Name.Should().Be(user.Name);
    }
    [Property]
    public static async Task EmptyDbGetUserDTOByIdReturnTest(NonWhiteSpaceString userId)
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        // Act
        var userDTO = await db.GetUserByIdAsync(userId.Item);

        // Assert
        userDTO.Should().BeNull();
    }

    [Property]
    public static async Task EmptyDbGetUserDTOByNameReturnTest(NonWhiteSpaceString name)
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        // Act
        var userDTO = await db.GetUserByUserNameAsync(name.Item);

        // Assert
        userDTO.Should().BeNull();
    }

    [Property]
    public static async Task CreateUserDTOTest(NonWhiteSpaceString username, MailAddress email, NonWhiteSpaceString name)
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        // Act
        var userDTO = await db.CreateUserAsync(username.Item, email.Address, name.Item);
        var nameUserDTO = await db.GetUserByUserNameAsync(username.Item);
        var idUserDTO = await db.GetUserByIdAsync(userDTO.Id);

        // Assert
        userDTO.Should().NotBeNull();
        userDTO.Name.Should().Be(name.Item);
        userDTO.Username.Should().Be(username.Item);

        nameUserDTO.Id.Should().Be(userDTO.Id);
        nameUserDTO.Username.Should().Be(userDTO.Username);
        nameUserDTO.Name.Should().Be(userDTO.Name);

        idUserDTO.Id.Should().Be(userDTO.Id);
        idUserDTO.Username.Should().Be(userDTO.Username);
        idUserDTO.Name.Should().Be(userDTO.Name);
    }

    [Property]
    public static async Task GeneratorTest()
    {
        // Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        // Act
        var user = (await UserGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];

        // Assert
        User foundUser = null;
        foreach (User u in dbOptions.BisonDbContext.Users)
        {
            if (u.Equals(user))
            {
                foundUser = u;
            }
            continue;
        }

        foundUser.Should().NotBeNull();
        foundUser.Should().Be(user);
    }
}
