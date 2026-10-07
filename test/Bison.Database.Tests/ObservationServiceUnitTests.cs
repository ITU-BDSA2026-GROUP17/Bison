using System.Net.NetworkInformation;

using Bison.Database.Exceptions;
using Bison.Database.Services;
using Bison.Models;

namespace Bison.Database.Tests;

public class ObservationServiceUnitTests
{
    [Property]
    public static async Task EmptyDbGetObsByIdTest(int obsId)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ObservationService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        var obs = await db.GetObservationByIdAsync(obsId);

        //Assert
        obs.Should().BeNull();
    }
    [Fact]
    public static async Task EmptyDbGetObsTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ObservationService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        var obs = await db.GetObservationsAsync();

        //Assert
        obs.Count.Should().Be(0);
    }
    [Property]
    public static async Task EmptyDbCreateObsTest(int userId, string description, string location)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ObservationService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        try
        {
            await db.CreateObservationAsync(userId, description, location);
            Assert.Fail("User does not exist, this should throw an error");
        }
        //Assert
        catch (UserDoesNotExistException e)
        {
            e.Message.Should().Be($"User with ID {userId} does not exist");
            e.UserId.Should().Be(userId);
        }
    }
    [Property]
    public static async Task CreateObsTest(string description, string location)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new ObservationService(repo);
        var user = await repo.CreateUser("testUser");

        //Act
        var obs = await db.CreateObservationAsync(user.Id, description, location);
        var idObs = await db.GetObservationByIdAsync(obs.Id);

        //Assert
        obs.Author.Id.Should().Be(user.Id);
        obs.Text.Should().Be(description);
        obs.Location.Should().Be(location);

        idObs.Id.Should().Be(obs.Id);
        idObs.Author.Id.Should().Be(user.Id);
        idObs.Text.Should().Be(description);
        idObs.Location.Should().Be(location);
    }
    [Fact]
    public static async Task PaginationWorksTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new ObservationService(repo);
        var user = await repo.CreateUser("testUser");
        List<Observation> list = [
            await db.CreateObservationAsync(user.Id, "test1", "bird Location1"),
            await db.CreateObservationAsync(user.Id, "test2", "bird Location2"),
            await db.CreateObservationAsync(user.Id, "test3", "bird Location3"),
            await db.CreateObservationAsync(user.Id, "test4", "bird Location4"),
            await db.CreateObservationAsync(user.Id, "test5", "bird Location5"),
        ];

        //Act
        var page1 = await db.GetObservationsAsync(4);
        var page2 = await db.GetObservationsAsync(4, 4);

        //Assert
        page1.Count.Should().Be(4);
        page2.Count.Should().Be(1);
    }
    [Fact]
    public static async Task PaginationWorksForGetByUserTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new ObservationService(repo);
        var user = await repo.CreateUser("testUser");
        List<Observation> list = [
            await db.CreateObservationAsync(user.Id, "test1", "bird Location1"),
            await db.CreateObservationAsync(user.Id, "test2", "bird Location2"),
            await db.CreateObservationAsync(user.Id, "test3", "bird Location3"),
            await db.CreateObservationAsync(user.Id, "test4", "bird Location4"),
            await db.CreateObservationAsync(user.Id, "test5", "bird Location5"),
        ];

        //Act
        var page1 = await db.GetObservationsByUserAsync(user.Id, 4);
        var page2 = await db.GetObservationsByUserAsync(user.Id, 4, 4);

        //Assert
        page1.Count.Should().Be(4);
        page2.Count.Should().Be(1);
    }
}
