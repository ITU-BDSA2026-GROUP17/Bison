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
        userDTO.Email.Should().Be(user.Email);
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
        var userDTO = await db.GetUserByNameAsync(user.Name);

        // Assert
        userDTO.Should().NotBeNull();
        userDTO.Id.Should().Be(user.Id);
        userDTO.Name.Should().Be(user.Name);
        userDTO.Email.Should().Be(user.Email);
    }
    [Property]
    public static async Task EmptyDbGetUserDTOByIdReturnTest(int userId)
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        // Act
        var userDTO = await db.GetUserByIdAsync(userId);

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
        var userDTO = await db.GetUserByNameAsync(name.Item);

        // Assert
        userDTO.Should().BeNull();
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

    [Fact]
    public async Task MapUserDTOToUserTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        var userId = (await UserGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0].Id;

        //Act
        var userDTO = await db.GetUserByIdAsync(userId);
        var user = await db.ToUserAsync(userDTO);

        //Assert
        userDTO.Should().NotBeNull();
        user.Should().NotBeNull();
        userDTO.Name.Should().Be(user.Name);
        userDTO.Email.Should().Be(user.Email);
        userDTO.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task MapUserToUserDTOTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var db = new UserService(new DatabaseRepository(dbOptions.BisonDbContext));

        var user = (await UserGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];

        //Act
        var userDTO = IUserService.ToUserDTO(user);

        //Assert
        userDTO.Should().NotBeNull();
        user.Should().NotBeNull();
        user.Name.Should().Be(userDTO.Name);
        user.Email.Should().Be(userDTO.Email);
        user.Id.Should().Be(userDTO.Id);
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
