using System.Net.Mail;

using Bison.Database.Services;

namespace Bison.Database.Tests;

public class IntegrationTests
{
    private static readonly Gen<NonWhiteSpaceString> StringGenerator = ArbMap.Default.GeneratorFor<NonWhiteSpaceString>();


    [Fact]
    public static async Task Test()
    {
        // Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var userService = new UserService(repo);
        var observationService = new ObservationService(repo);
        var commentService = new CommentService(repo);
        var proposalService = new ProposalService(repo);
        var taxonGenerator = (await ProposalServiceIntegrationTests.TaxonGenerator(dbOptions.BisonDbContext)).Generator;

        var users = await Task.WhenAll((
            from username in StringGenerator
            from email in ArbMap.Default.GeneratorFor<MailAddress>()
            from name in StringGenerator
            select userService.CreateUserAsync(username.Item, email.Address, name.Item)
        ).Sample(5));
        var observations = await Task.WhenAll((
            from author in Gen.Elements(users)
            from taxon in taxonGenerator
            from text in StringGenerator
            select observationService.CreateObservationAsync(author.Id, taxon.DWC_TaxonId, text.Item)
        ).Sample(25));
        var comments = (await Task.WhenAll((
            from author in Gen.Elements(users)
            from obs in Gen.Elements(observations)
            from text in StringGenerator
            select commentService.CreateCommentAsync(author.Id, obs.Id, text.Item)
        ).Sample(50))).GroupBy(c => c.ObservationId).ToDictionary(k => k.Key, v => v.ToList());
        var proposals = (await Task.WhenAll((
            from author in Gen.Elements(users)
            from obs in Gen.Elements(observations)
            from taxon in taxonGenerator
            from text in StringGenerator
            select proposalService.CreateProposalAsync(author.Id, obs.Id, taxon.DWC_TaxonId, text.Item)
        ).Sample(35))).GroupBy(p => p.ObservationId).ToDictionary(k => k.Key, v => v.ToList());

        // Act
        var gottenUsers = await Task.WhenAll(
            users.Select(u => userService.GetUserByIdAsync(u.Id))
        );
        var gottenObservations = await Task.WhenAll(
            observations.Select(o => observationService.GetObservationByIdAsync(o.Id))
        );
        var gottenComments = (await Task.WhenAll(
            comments.Select(k => commentService.GetCommentsByObservationIdAsync(k.Key))
        )).ToDictionary(k => k[0].ObservationId, v => v);
        var gottenProposals = (await Task.WhenAll(
            proposals.Select(k => proposalService.GetProposalsByObservationIdAsync(k.Key))
        )).ToDictionary(k => k[0].ObservationId, v => v);

        // Assert
        gottenUsers.ScrambledEquals(users).Should().BeTrue();
        gottenObservations.ScrambledEquals(observations).Should().BeTrue();
        gottenComments.All(kvp => kvp.Value.ScrambledEquals(comments[kvp.Key])).Should().BeTrue();
        gottenProposals.All(kvp => kvp.Value.ScrambledEquals(proposals[kvp.Key])).Should().BeTrue();
    }
}

public static class ListExtensions
{
    public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2) where T : IEquatable<T>
    {
        var count = new Dictionary<T, int>();
        foreach (T item in list1)
        {
            if (count.TryGetValue(item, out int value))
            {
                count[item] = ++value;
            }
            else
            {
                count.Add(item, 1);
            }
        }
        foreach (T item in list2)
        {
            if (count.TryGetValue(item, out int value))
            {
                count[item] = --value;
            }
            else
            {
                return false;
            }
        }
        return count.Values.All(c => c == 0);
    }
}
