using Bison.Database.Exceptions;
using Bison.Database.Services;
using Bison.Models;

namespace Bison.Database.Tests;

public class ProposalServiceUnitTests
{
    private static readonly Taxonomies Taxonomies = new();
    public static Arbitrary<string> TaxonIdGenerator() =>
        (from i in Gen.Choose(0, Taxonomies.TaxonomiesList.Count - 1)
         select Taxonomies.TaxonomiesList[i].TaxonID).ToArbitrary();

    [Property]
    public static async Task EmptyDbGetProposalsByObsIdTest(int obsId)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ProposalService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        var ppl = await db.GetProposalsByObservationIdAsync(obsId);

        //Assert
        ppl.Should().NotBeNull();
        ppl.Count.Should().Be(0);
    }
    [Property]
    public static async Task EmptyDbCreateProposalWithInvalidTaxonIdTest(int userId, int obsId, string invalidTaxonId)
    {
        foreach (var taxon in Taxonomies.TaxonomiesList)
        {
            if (taxon.TaxonID == invalidTaxonId)
            {
                //if the invalidTaxon is actually valid, this run is invalid, because our assumption is broken
                return;
            }
        }
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ProposalService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        try
        {
            await db.CreateProposalAsync(userId, obsId, invalidTaxonId);
            Assert.Fail("Invalid taxon Id, should throw an exception");
        }
        //Assert
        catch (TaxonIdDoesNotExistException e)
        {
            e.Message.Should().Be($"Taxon with ID {invalidTaxonId} does not exist");
            e.TaxonId.Should().Be(invalidTaxonId);
        }
    }
    [Property(Arbitrary = new[] { typeof(ProposalServiceUnitTests) })]
    public static async Task EmptyDbCreateProposalWithValidTaxonIdTest(int userId, int obsId, string taxonId)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ProposalService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        try
        {
            await db.CreateProposalAsync(userId, obsId, taxonId);
            Assert.Fail("No user, should throw an exception");
        }
        //Assert
        catch (UserDoesNotExistException e)
        {
            e.Message.Should().Be($"User with ID {userId} does not exist");
            e.UserId.Should().Be(userId);
        }
    }

    [Property(Arbitrary = new[] { typeof(ProposalServiceUnitTests) })]
    public static async Task DbWithUserCreateProposalTest(int obsId, string taxonId)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName");
        var db = new ProposalService(repo);

        //Act
        try
        {
            await db.CreateProposalAsync(user.Id, obsId, taxonId);
            Assert.Fail("No Observation, should throw an exception");
        }
        //Assert
        catch (ObservationDoesNotExistException e)
        {
            e.Message.Should().Be($"Observation with ID {obsId} does not exist");
            e.ObservationId.Should().Be(obsId);
        }
    }
    [Property(Arbitrary = new[] { typeof(ProposalServiceUnitTests) })]
    public static async Task CreateProposalTest(string taxonId)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName");
        var obs = await repo.CreateObservation(user.Id, "TestObs", "TestLocation");
        var db = new ProposalService(repo);

        //Act
        var ppl = await db.CreateProposalAsync(user.Id, obs.Id, taxonId);
        var gottenPpl = await db.GetProposalsByObservationIdAsync(obs.Id);


        //Assert
        ppl.Should().NotBeNull();
        ppl.Author.Id.Should().Be(user.Id);
        ppl.Observation.Id.Should().Be(obs.Id);
        ppl.TaxonId.Should().Be(taxonId);

        gottenPpl.Should().NotBeNull();
        gottenPpl.Count.Should().Be(1);
        gottenPpl[0].Should().NotBeNull();
        gottenPpl[0].Id.Should().Be(ppl.Id);
        gottenPpl[0].Author.Id.Should().Be(user.Id);
        gottenPpl[0].Observation.Id.Should().Be(obs.Id);
        gottenPpl[0].TaxonId.Should().Be(taxonId);
    }
    public static async Task PaginationWorksTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName");
        var obs = await repo.CreateObservation(user.Id, "TestObs", "TestLocation");
        var db = new ProposalService(repo);
        var taxonIds = TaxonIdGenerator().Generator.Sample(5);
        List<Proposal> list = [
            await db.CreateProposalAsync(user.Id, obs.Id, taxonIds[0]),
            await db.CreateProposalAsync(user.Id, obs.Id, taxonIds[1]),
            await db.CreateProposalAsync(user.Id, obs.Id, taxonIds[2]),
            await db.CreateProposalAsync(user.Id, obs.Id, taxonIds[3]),
            await db.CreateProposalAsync(user.Id, obs.Id, taxonIds[4])
        ];

        //Act
        var page1 = await db.GetProposalsByObservationIdAsync(obs.Id, 4);
        var page2 = await db.GetProposalsByObservationIdAsync(obs.Id, 2, 4);

        //Assert
        page1.Count.Should().Be(4);
        page2.Count.Should().Be(1);
        page1.Select(ppl => ppl.Id).Should().BeInDescendingOrder();
    }
}
