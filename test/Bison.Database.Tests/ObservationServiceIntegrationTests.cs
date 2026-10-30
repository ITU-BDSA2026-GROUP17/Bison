using Bison.Database.Exceptions;
using Bison.Database.Services;

namespace Bison.Database.Tests;

public class ObservationServiceIntegrationTests
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
    public static async Task EmptyDbCreateObsTest(int userId, NonWhiteSpaceString description)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var db = new ObservationService(new DatabaseRepository(dbOptions.BisonDbContext));
        var taxon = (await ProposalServiceIntegrationTests.TaxonGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];

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
    [Property]
    public static async Task EmptyDbCreateObsTestInvalidTaxonId(NonWhiteSpaceString taxonId, NonWhiteSpaceString description)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new ObservationService(repo);
        var user = await repo.CreateUser("test", "test@example.com");

        if (await repo.GetTaxonByDWCId(taxonId.Item) is not null)
            return; // make sure that the "taxonId" is invalid.

        //Act
        try
        {
            await db.CreateObservationAsync(user.Id, taxonId.Item, description.Item);
            Assert.Fail("Taxon does not exist, this should throw an error");
        }
        //Assert
        catch (TaxonIdDoesNotExistException err)
        {
            err.Message.Should().Be($"Taxon with ID {taxonId.Item} does not exist");
            err.TaxonId.Should().Be(taxonId.Item);
        }
    }
    [Property]
    public static async Task CreateObsTest(NonWhiteSpaceString description)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var taxon = (await ProposalServiceIntegrationTests.TaxonGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new ObservationService(repo);
        var user = await repo.CreateUser("testUser", "test@example.com");

        //Act
        var obs = await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, description.Item);
        var idObs = await db.GetObservationByIdAsync(obs.Id);

        //Assert
        obs.Author.Id.Should().Be(user.Id);
        obs.Description.Should().Be(description.Item);
        obs.TaxonName.Should().Be(taxon.VernacularName);

        idObs.Id.Should().Be(obs.Id);
        idObs.Author.Id.Should().Be(user.Id);
        idObs.Description.Should().Be(description.Item);
        idObs.TaxonName.Should().Be(taxon.VernacularName);
    }
    [Fact]
    public static async Task PaginationWorksTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var taxon = (await ProposalServiceIntegrationTests.TaxonGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new ObservationService(repo);
        var user = await repo.CreateUser("testUser", "test@example.com");
        List<ObservationDTO> list = [
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
        dbOptions.SeedTaxonsOnly();
        var taxon = (await ProposalServiceIntegrationTests.TaxonGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new ObservationService(repo);
        var user = await repo.CreateUser("testUser", "test@example.com");
        List<ObservationDTO> list = [
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test1"),
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test2"),
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test3"),
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test4"),
            await db.CreateObservationAsync(user.Id, taxon.DWC_TaxonId, "test5"),
        ];

        //Act
        var page1 = await db.GetObservationsByUserAsync(user.Id, 4);
        var page2 = await db.GetObservationsByUserAsync(user.Id, 2, 4);
        var allTogether = await db.GetObservationsByUserAsync(user.Id);

        //Assert
        page1.Count.Should().Be(4);
        page2.Count.Should().Be(1);
        page1.Select(obs => obs.Id).Should().BeInDescendingOrder();
        allTogether.Count.Should().Be(5);
        allTogether.Select(obs => obs.Id).Should().BeInDescendingOrder();
    }

    [Property]
    public static async Task ErrorThrownOnInvalidTaxonFilter(NonWhiteSpaceString taxonId)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var db = new ObservationService(repo);

        if (await repo.GetTaxonByDWCId(taxonId.Item) is not null)
            return; // make sure that the "taxonId" is invalid.

        //Act
        try
        {
            await db.GetObservationsByTaxonAsync(taxonId.Item);
            Assert.Fail("Should've thrown an exception already");
        }
        //Assert
        catch (TaxonIdDoesNotExistException err)
        {
            err.Message.Should().Be($"Taxon with ID {taxonId.Item} does not exist");
            err.TaxonId.Should().Be(taxonId.Item);
        }
    }
    [Fact]
    public static async Task AllObservationsAreReturnedOnTaxonFilter()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var db = new ObservationService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        var obss = await db.GetObservationsByTaxonAsync(
            "MSTSNM:Arter:3e4e67e4-f785-ea11-aa77-501ac539d1ea" // pelecaniformes
        );

        //Assert
        obss.Count.Should().Be(410);
    }
}
