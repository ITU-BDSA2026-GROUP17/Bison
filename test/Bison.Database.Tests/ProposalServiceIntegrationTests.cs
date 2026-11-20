using Bison.Database.Exceptions;
using Bison.Database.Services;
using Bison.Models;

using Microsoft.EntityFrameworkCore;

namespace Bison.Database.Tests;

public class ProposalServiceIntegrationTests
{
    public static async Task<Arbitrary<Taxon>> TaxonGenerator(BisonDbContext dbContext) =>
        Gen.Elements((IEnumerable<Taxon>)await dbContext.Taxons.ToListAsync()).ToArbitrary();

    [Property]
    public static async Task EmptyDbGetProposalsByObsIdTest(int obsId)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var db = new ProposalService(new DatabaseRepository(dbOptions.BisonDbContext));

        //Act
        var ppl = await db.GetProposalsByObservationIdAsync(obsId);

        //Assert
        ppl.Should().NotBeNull();
        ppl.Count.Should().Be(0);
    }
    [Property]
    public static async Task EmptyDbCreateProposalWithInvalidTaxonIdTest(NonWhiteSpaceString userId, int obsId, string invalidTaxonId)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var db = new ProposalService(new DatabaseRepository(dbOptions.BisonDbContext));

        if (await dbOptions.BisonDbContext.Taxons.Where(t => t.DWC_TaxonId == invalidTaxonId).FirstOrDefaultAsync() is not null)
            return;

        //Act
        try
        {
            await db.CreateProposalAsync(userId.Item, obsId, invalidTaxonId, "prop1");
            Assert.Fail("No user, should throw an exception");
        }
        //Assert
        catch (UserDoesNotExistException e)
        {
            e.Message.Should().Be($"User with ID {userId.Item} does not exist");
            e.UserId.Should().Be(userId.Item);
        }
    }
    [Property]
    public static async Task EmptyDbCreateProposalWithValidTaxonIdTest(NonWhiteSpaceString userId, int obsId)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var db = new ProposalService(new DatabaseRepository(dbOptions.BisonDbContext));

        var taxon = (await TaxonGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];
        //Act
        try
        {
            await db.CreateProposalAsync(userId.Item, obsId, taxon.DWC_TaxonId, "prop1");
            Assert.Fail("No user, should throw an exception");
        }
        //Assert
        catch (UserDoesNotExistException e)
        {
            e.Message.Should().Be($"User with ID {userId.Item} does not exist");
            e.UserId.Should().Be(userId.Item);
        }
    }
    [Property]
    public static async Task DbWithUserCreateProposalTest(int obsId)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName", "test@example.com", "test");
        var db = new ProposalService(repo);

        var taxon = (await TaxonGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];

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
    [Property]
    public static async Task DbCreateProposalWithInvalidTaxonIdTest(string invalidTaxonId)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var taxon = (await TaxonGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];

        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName", "test@example.com", "test");
        var obs = await repo.CreateObservation(user.Id, taxon.DWC_TaxonId, "TestLocation");
        var db = new ProposalService(repo);

        if (await dbOptions.BisonDbContext.Taxons.Where(t => t.DWC_TaxonId == invalidTaxonId).FirstOrDefaultAsync() is not null)
            return;

        //Act
        try
        {
            await db.CreateProposalAsync(user.Id, obs.Id, invalidTaxonId, "prop1");
            Assert.Fail("Invalid taxon Id, should throw an exception");
        }
        //Assert
        catch (TaxonIdDoesNotExistException e)
        {
            e.Message.Should().Be($"Taxon with ID {invalidTaxonId} does not exist");
            e.TaxonId.Should().Be(invalidTaxonId);
        }
    }
    [Property]
    public static async Task CreateProposalOnCommentTest(NonWhiteSpaceString comment)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var taxon = (await ProposalServiceIntegrationTests.TaxonGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName", "test@example.com", "test");
        var obs = await repo.CreateObservation(user.Id, taxon.DWC_TaxonId, "TestLocation");
        var cmt = await repo.CreateComment(user.Id, obs.Id, comment.Item);
        var db = new ProposalService(repo);

        //Act
        try
        {
            var ppl = await db.CreateProposalAsync(user.Id, cmt.Id, taxon.DWC_TaxonId, comment.Item);
            Assert.Fail("Invalid observation Id, should throw an exception");
        }
        //Assert
        catch (ObservationDoesNotExistException e)
        {
            e.Message.Should().Be($"Observation with ID {cmt.Id} does not exist");
            e.ObservationId.Should().Be(cmt.Id);
        }
    }
    [Property]
    public static async Task CreateProposalTest(NonWhiteSpaceString text)
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedDatabase();
        var taxon = (await TaxonGenerator(dbOptions.BisonDbContext)).Generator.Sample(1)[0];

        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName", "test@example.com", "test");
        var obs = await repo.CreateObservation(user.Id, taxon.DWC_TaxonId, "TestLocation");
        var db = new ProposalService(repo);
        var userDB = new UserService(repo);
        var obsDB = new ObservationService(repo);

        //Act
        var pplDTO = await db.CreateProposalAsync(user.Id, obs.Id, taxon.DWC_TaxonId, text.Item);
        var ppl = await db.ToProposalAsync(pplDTO);
        var gottenPpls = await db.GetProposalsByObservationIdAsync(obs.Id);
        var gottenPpl = await db.ToProposalAsync(gottenPpls[0]);

        //Assert
        pplDTO.Should().NotBeNull();

        ppl.Author.Id.Should().Be(user.Id);
        ppl.Observation.Id.Should().Be(obs.Id);
        ppl.Observation.Id.Should().Be(obs.Id);
        ppl.Taxon.Id.Should().Be(taxon.Id);
        ppl.Text.Should().Be(text.Item);

        gottenPpls.Should().NotBeNull();
        gottenPpls.Count.Should().Be(1);
        gottenPpl.Should().NotBeNull();
        gottenPpl.Id.Should().Be(pplDTO.Id);
        gottenPpl.Author.Id.Should().Be(user.Id);
        gottenPpl.Observation.Id.Should().Be(obs.Id);
        gottenPpl.Taxon.Id.Should().Be(taxon.Id);
        gottenPpl.Text.Should().Be(text.Item);
    }
    [Fact]
    public static async Task PaginationWorksTest()
    {
        //Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var taxonIds = (await TaxonGenerator(dbOptions.BisonDbContext)).Generator.Sample(5);
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var user = await repo.CreateUser("testName", "test@example.com", "test");
        var obs = await repo.CreateObservation(user.Id, taxonIds[0].DWC_TaxonId, "TestLocation");
        var db = new ProposalService(repo);
        List<ProposalDTO> list = [
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
