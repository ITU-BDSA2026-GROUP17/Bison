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
    [Property(Arbitrary = new[] { typeof(ProposalServiceUnitTests) })]
    public static async Task EmptyDbCreateObsTest(int userId, Taxon taxon, NonWhiteSpaceString description)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ObservationService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        try
        {
            await db.CreateObservationAsync(userId, taxon.DWC_TaxonId, description.Item);
            Assert.Fail("User does not exist, this should throw an error");
        }
        //Assert
        catch (UserDoesNotExistException e)
        {
            e.Message.Should().Be($"User with ID {userId} does not exist");
            e.UserId.Should().Be(userId);
        }
    }
    [Property(Arbitrary = new[] { typeof(ProposalServiceUnitTests) })]
    public static async Task CreateObsTest(Taxon taxon, NonWhiteSpaceString description)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new ObservationService(repo);
        var user = await repo.CreateUser("testUser", "test@example.com");

        //Act
        var obs = await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, description.Item);
        var idObs = await db.GetObservationByIdAsync(obs.Id);

        //Assert
        obs.Author.Id.Should().Be(user.Id);
        obs.Text.Should().Be(description.Item);
        obs.Taxon.Id.Should().Be(taxon.Id);

        idObs.Id.Should().Be(obs.Id);
        idObs.Author.Id.Should().Be(user.Id);
        idObs.Text.Should().Be(description.Item);
        idObs.Taxon.Id.Should().Be(taxon.Id);
    }
    [Fact]
    public static async Task PaginationWorksTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var taxon = (await ProposalServiceUnitTests.TaxonIdGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new ObservationService(repo);
        var user = await repo.CreateUser("testUser", "test@example.com");
        List<Observation> list = [
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test1"),
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test2"),
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test3"),
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test4"),
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test5"),
        ];

        //Act
        var page1 = await db.GetObservationsAsync(4);
        var page2 = await db.GetObservationsAsync(2, 4);

        //Assert
        page1.Count.Should().Be(4);
        page2.Count.Should().Be(1);
        page1.Select(obs => obs.Id).Should().BeInDescendingOrder();
    }
    [Fact]
    public static async Task PaginationWorksForGetByUserTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var taxon = (await ProposalServiceUnitTests.TaxonIdGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new ObservationService(repo);
        var user = await repo.CreateUser("testUser", "test@example.com");
        List<Observation> list = [
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test1"),
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test2"),
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test3"),
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test4"),
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test5"),
        ];

        //Act
        var page1 = await db.GetObservationsByUserAsync(user.Id, 4);
        var page2 = await db.GetObservationsByUserAsync(user.Id, 2, 4);

        //Assert
        page1.Count.Should().Be(4);
        page2.Count.Should().Be(1);
        page1.Select(obs => obs.Id).Should().BeInDescendingOrder();
    }
}
