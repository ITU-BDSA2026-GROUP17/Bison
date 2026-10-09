using Bison.Database.Services;
using Bison.Models;

using CsvHelper.Configuration.Attributes;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Bison.Database.Tests;

public class IntegrationTests
{
    private static readonly Gen<NonEmptyString> StringGenerator = ArbMap.Default.GeneratorFor<NonEmptyString>();


    [Fact]
    public static async Task Test()
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var repo = new DatabaseRepository(dbOptions.BisonDbContext);
        var userService = new UserService(repo);
        var observationService = new ObservationService(repo);
        var commentService = new CommentService(repo);
        var proposalService = new ProposalService(repo);

        var users = await Task.WhenAll((
            from name in StringGenerator
            select userService.CreateUserAsync(name.ToString())
        ).Sample(5));
        var observations = await Task.WhenAll((
            from author in Gen.Elements(users)
            from text in StringGenerator
            from location in StringGenerator
            select observationService.CreateObservationAsync(author.Id, text.ToString(), location.ToString())
        ).Sample(25));
        var comments = (await Task.WhenAll((
            from author in Gen.Elements(users)
            from obs in Gen.Elements(observations)
            from text in StringGenerator
            select commentService.CreateCommentAsync(author.Id, obs.Id, text.ToString())
        ).Sample(50))).GroupBy(c => c.Observation.Id).ToDictionary(k => k.Key, v => v.ToList());
        var proposals = (await Task.WhenAll((
            from author in Gen.Elements(users)
            from obs in Gen.Elements(observations)
            from taxon in ProposalServiceUnitTests.TaxonIdGenerator().Generator
            select proposalService.CreateProposalAsync(author.Id, obs.Id, taxon)
        ).Sample(35))).GroupBy(p => p.Observation.Id).ToDictionary(k => k.Key, v => v.ToList());

        // Act
        var gottenUsers = await Task.WhenAll(
            users.Select(u => userService.GetUserByIdAsync(u.Id))
        );
        var gottenObservations = await Task.WhenAll(
            observations.Select(o => observationService.GetObservationByIdAsync(o.Id))
        );
        var gottenComments = (await Task.WhenAll(
            comments.Select(k => commentService.GetCommentsByObservationIdAsync(k.Key))
        )).ToDictionary(k => k[0].Observation.Id, v => v);
        var gottenProposals = (await Task.WhenAll(
            proposals.Select(k => proposalService.GetProposalsByObservationIdAsync(k.Key))
        )).ToDictionary(k => k[0].Observation.Id, v => v);

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
