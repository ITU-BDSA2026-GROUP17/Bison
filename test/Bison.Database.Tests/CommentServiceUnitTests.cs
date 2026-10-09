using Bison.Database.Exceptions;
using Bison.Database.Services;
using Bison.Models;

namespace Bison.Database.Tests;

public class CommentServiceUnitTests
{
    [Property]
    public static async Task EmptyDbGetCommentsByObsIdTest(int obsId)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new CommentService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        var com = await db.GetCommentsByObservationIdAsync(obsId);

        //Assert
        com.Should().NotBeNull();
        com.Count.Should().Be(0);
    }
    [Property]
    public static async Task EmptyDbCreateCommentTest(int userId, int obsId, string comment)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new CommentService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        try
        {
            await db.CreateCommentAsync(userId, obsId, comment);
            Assert.Fail("No user, should throw an exception");
        }
        //Assert
        catch (UserDoesNotExistException e)
        {
            e.Message.Should().Be($"User with ID {userId} does not exist");
            e.UserId.Should().Be(userId);
        }
    }
    [Property]
    public static async Task DbWithUserCreateCommentTest(int obsId, string comment)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName");
        var db = new CommentService(repo);

        //Act
        try
        {
            await db.CreateCommentAsync(user.Id, obsId, comment);
            Assert.Fail("No Observation, should throw an exception");
        }
        //Assert
        catch (ObservationDoesNotExistException e)
        {
            e.Message.Should().Be($"Observation with ID {obsId} does not exist");
            e.ObservationId.Should().Be(obsId);
        }
    }
    [Property]
    public static async Task CreateCommentTest(string comment)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName");
        var obs = await repo.CreateObservation(user.Id, "TestObs", "TestLocation");
        var db = new CommentService(repo);

        //Act
        var com = await db.CreateCommentAsync(user.Id, obs.Id, comment);
        var gottenCom = await db.GetCommentsByObservationIdAsync(obs.Id);


        //Assert
        com.Should().NotBeNull();
        com.Author.Id.Should().Be(user.Id);
        com.Observation.Id.Should().Be(obs.Id);
        com.Text.Should().Be(comment);

        gottenCom.Should().NotBeNull();
        gottenCom.Count.Should().Be(1);
        gottenCom[0].Should().NotBeNull();
        gottenCom[0].Id.Should().Be(com.Id);
        gottenCom[0].Author.Id.Should().Be(user.Id);
        gottenCom[0].Observation.Id.Should().Be(obs.Id);
        gottenCom[0].Text.Should().Be(comment);
    }
    public static async Task PaginationWorksTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName");
        var obs = await repo.CreateObservation(user.Id, "TestObs", "TestLocation");
        var db = new CommentService(repo);
        List<Comment> list = [
            await db.CreateCommentAsync(user.Id, obs.Id, "com1"),
            await db.CreateCommentAsync(user.Id, obs.Id, "com2"),
            await db.CreateCommentAsync(user.Id, obs.Id, "com3"),
            await db.CreateCommentAsync(user.Id, obs.Id, "com4"),
            await db.CreateCommentAsync(user.Id, obs.Id, "com5")
        ];

        //Act
        var page1 = await db.GetCommentsByObservationIdAsync(obs.Id, 4);
        var page2 = await db.GetCommentsByObservationIdAsync(obs.Id, 2, 4);

        //Assert
        page1.Count.Should().Be(4);
        page2.Count.Should().Be(1);
        page1.Select(com => com.Id).Should().BeInDescendingOrder();
    }
}
