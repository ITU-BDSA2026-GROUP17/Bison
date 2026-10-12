using Bison.Database.Exceptions;
using Bison.Database.Services;
using Bison.Models;

using Microsoft.EntityFrameworkCore;

namespace Bison.Database.Tests;

public class ProposalServiceUnitTests
{
    public static async Task<Arbitrary<Taxon>> TaxonIdGenerator(BisonDbContext dbContext) =>
        Gen.Elements((IEnumerable<Taxon>)await dbContext.Taxons.ToListAsync()).ToArbitrary();

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
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ProposalService(new DatabaseRepository(dbOptions.BisonDbContext));
        if (await dbOptions.BisonDbContext.Taxons.Where(t => t.DWC_TaxonId == invalidTaxonId).FirstAsync() is not null)
            return;

        //Act
        try
        {
            await db.CreateProposalAsync(userId, obsId, invalidTaxonId, "prop1");
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
    public static async Task EmptyDbCreateProposalWithValidTaxonIdTest(int userId, int obsId, Taxon taxon)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var db = new ProposalService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        try
        {
            await db.CreateProposalAsync(userId, obsId, taxon.DWC_TaxonId, "prop1");
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
    public static async Task DbWithUserCreateProposalTest(int obsId, Taxon taxon)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName", "test@example.com");
        var db = new ProposalService(repo);

        //Act
        try
        {
            await db.CreateProposalAsync(user.Id, obsId, taxon.DWC_TaxonId, "prop1");
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
    public static async Task CreateProposalTest(Taxon taxon, NonWhiteSpaceString text)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName", "test@example.com");
        var obs = await repo.CreateObservation(user.Id, "TestObs", "TestLocation");
        var db = new ProposalService(repo);

        //Act
        var ppl = await db.CreateProposalAsync(user.Id, obs.Id, taxon.DWC_TaxonId, text.Item);
        var gottenPpl = await db.GetProposalsByObservationIdAsync(obs.Id);


        //Assert
        ppl.Should().NotBeNull();
        ppl.Author.Id.Should().Be(user.Id);
        ppl.Observation.Id.Should().Be(obs.Id);
        ppl.Taxon.Id.Should().Be(taxon.Id);
        ppl.Text.Should().Be(text.Item);

        gottenPpl.Should().NotBeNull();
        gottenPpl.Count.Should().Be(1);
        gottenPpl[0].Should().NotBeNull();
        gottenPpl[0].Id.Should().Be(ppl.Id);
        gottenPpl[0].Author.Id.Should().Be(user.Id);
        gottenPpl[0].Observation.Id.Should().Be(obs.Id);
        gottenPpl[0].Taxon.Id.Should().Be(taxon.Id);
        gottenPpl[0].Text.Should().Be(text.Item);
    }
    public static async Task PaginationWorksTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        var taxonIds = (await TaxonIdGenerator(dbOptions.BisonDbContext)).Generator.Sample(5);
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName", "test@example.com");
        var obs = await repo.CreateObservation(user.Id, taxonIds[0].DWC_TaxonId, "TestLocation");
        var db = new ProposalService(repo);
        List<Proposal> list = [
            await db.CreateProposalAsync(user.Id, obs.Id, taxonIds[0].DWC_TaxonId, "prop1"),
            await db.CreateProposalAsync(user.Id, obs.Id, taxonIds[1].DWC_TaxonId, "prop2"),
            await db.CreateProposalAsync(user.Id, obs.Id, taxonIds[2].DWC_TaxonId, "prop3"),
            await db.CreateProposalAsync(user.Id, obs.Id, taxonIds[3].DWC_TaxonId, "prop4"),
            await db.CreateProposalAsync(user.Id, obs.Id, taxonIds[4].DWC_TaxonId, "prop5")
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
