using Bison.Database.Exceptions;
using Bison.Database.Services;

namespace Bison.Database.Tests;

public class CommentServiceIntegrationTests
{
    [Property]
    public static async Task EmptyDbGetCommentsByObsIdTest(int obsId)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var db = new CommentService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        var com = await db.GetCommentsByObservationIdAsync(obsId);

        //Assert
        com.Should().NotBeNull();
        com.Count.Should().Be(0);
    }
    [Property]
    public static async Task EmptyDbCreateCommentTest(int userId, int obsId, NonWhiteSpaceString comment)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var db = new CommentService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        try
        {
            await db.CreateCommentAsync(userId, obsId, comment.Item);
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
    public static async Task DbWithUserCreateCommentTest(int obsId, NonWhiteSpaceString comment)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName", "test@example.com");
        var db = new CommentService(repo);

        //Act
        try
        {
            await db.CreateCommentAsync(user.Id, obsId, comment.Item);
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
    public static async Task CreateCommentTest(NonWhiteSpaceString comment)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var taxon = (await ProposalServiceIntegrationTests.TaxonGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName", "test@example.com");
        var obs = await repo.CreateObservation(user.Id, taxon.DWC_TaxonId, "TestLocation");
        var db = new CommentService(repo);

        //Act
        var com = await db.CreateCommentAsync(user.Id, obs.Id, comment.Item);
        var gottenCom = await db.GetCommentsByObservationIdAsync(obs.Id);

        //Assert
        com.Should().NotBeNull();
        com.Author.Id.Should().Be(user.Id);
        com.ObservationId.Should().Be(obs.Id);
        com.Comment.Should().Be(comment.Item);

        gottenCom.Should().NotBeNull();
        gottenCom.Count.Should().Be(1);
        gottenCom[0].Should().NotBeNull();
        gottenCom[0].Id.Should().Be(com.Id);
        gottenCom[0].Author.Id.Should().Be(user.Id);
        gottenCom[0].ObservationId.Should().Be(obs.Id);
        gottenCom[0].Comment.Should().Be(comment.Item);
    }
    [Property]
    public static async Task CreateCommentOnCommentTest(NonWhiteSpaceString comment)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var taxon = (await ProposalServiceIntegrationTests.TaxonGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName", "test@example.com");
        var obs = await repo.CreateObservation(user.Id, taxon.DWC_TaxonId, "TestLocation");
        var cmt = await repo.CreateComment(user.Id, obs.Id, comment.Item);
        var db = new CommentService(repo);

        //Act
        try
        {
            var com = await db.CreateCommentAsync(user.Id, cmt.Id, comment.Item);
            Assert.Fail("Invalid observation Id, should throw an exception");
        }
        //Assert
        catch (ObservationDoesNotExistException e)
        {
            e.Message.Should().Be($"Observation with ID {cmt.Id} does not exist");
            e.ObservationId.Should().Be(cmt.Id);
        }
    }
    [Fact]
    public static async Task PaginationWorksTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var taxon = (await ProposalServiceIntegrationTests.TaxonGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName", "test@example.com");
        var obs = await repo.CreateObservation(user.Id, taxon.DWC_TaxonId, "TestLocation");
        var db = new CommentService(repo);
        List<CommentDTO> list = [
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
