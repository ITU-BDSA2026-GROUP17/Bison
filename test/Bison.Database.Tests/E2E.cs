namespace Bison.Database.Tests;

using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;

using Bison.Models;
using Bison.Utilities;

using FluentAssertions;

using FsCheck;
using FsCheck.Fluent;

public class E2E
{
    private static readonly Taxonomies Taxonomies = new();

    /// <summary>
    /// Creates a generator over valid ids in <see cref="Database.Taxonomies" />.
    /// </summary>
    public static Arbitrary<string> TaxonIdGenerator()
    {
        Gen<string> taxonGenerator = Gen.Choose(0, Taxonomies.TaxonomiesList.Count - 1)
        .Select(i => Taxonomies.TaxonomiesList[i].TaxonID);
        return taxonGenerator.ToArbitrary();
    }
    /// <summary>
    /// Creates a generator over valid UNIX <see cref="DateTime" />s.
    /// </summary>
    public static Arbitrary<DateTime> DateGenerator()
    {
        DateTime minDate = new(1970, 1, 1, 0, 0, 0);
        DateTime maxDate = new(2038, 1, 19, 3, 14, 7);
        int totalSecRange = (int)(maxDate - minDate).TotalSeconds;

        // Gen.Choose picks a random integer, which we map back to a DateTime
        Gen<DateTime> dateGenerator = Gen.Choose(0, totalSecRange)
            .Select(secs => minDate.AddSeconds(secs));

        return dateGenerator.ToArbitrary();
    }

    public static Process StartDatabase()
    {
        var process = Process.Start(new ProcessStartInfo()
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            FileName = "dotnet",
            Arguments = "run --project src/Bison.Database",
            WorkingDirectory = "../../../../../"
        });
        Assert.True(process is not null);

        while (true)
        {
            var output = process.StandardOutput.ReadLine();
            if (output == null || output.Contains("localhost"))
            {
                break;
            }
        }

        return process;
    }
    static readonly HttpClient DBClient = new();
    const string DATABASE_URI = "http://localhost:5001";

    internal static void InitializeDBClient()
    {
        DBClient.DefaultRequestHeaders.Accept.Clear();
        DBClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        DBClient.BaseAddress = new(DATABASE_URI);
    }

    /// <summary>
    /// Tests if two lists are equal regardless of order.
    /// </summary>
    /// <param name="list1">one of the lists to compare</param>
    /// <param name="list2">the other of the lists to compare</param>
    /// <returns>
    /// <c>true</c> if <c>list1</c> and <c>list2</c> contains the same elements in some order
    /// </returns>
    public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
    {
        var cnt = new Dictionary<T, int>();
        foreach (T s in list1)
        {
            if (cnt.TryGetValue(s, out int value))
            {
                cnt[s] = ++value;
            }
            else
            {
                cnt.Add(s, 1);
            }
        }
        foreach (T s in list2)
        {
            if (cnt.TryGetValue(s, out int value))
            {
                cnt[s] = --value;
            }
            else
            {
                return false;
            }
        }
        return cnt.Values.All(c => c == 0);
    }

    [Fact]
    public static async Task TestRandomRecordsAreSavedProperly()
    {
        InitializeDBClient();
        using var db = StartDatabase();
        var string_gen = ArbMap.Default.GeneratorFor<NonEmptyString>();
        var date_gen = DateGenerator().Generator;

        // generate 25 random observations
        var observations = (
            from author in string_gen
            from obs in string_gen
            from location in string_gen
            from timestamp in date_gen
            select new ObservationRecord()
            {
                Author = author.ToString(),
                Observation = obs.ToString(),
                Location = location.ToString(),
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(timestamp),
            }
        ).Sample(25);

        // save them in the db and update their id to the id given by the database
        foreach (var observation in observations)
        {
            var req = await DBClient.PostAsJsonAsync("/observation", observation);
            req.EnsureSuccessStatusCode();
            var res = await req.Content.ReadFromJsonAsync<PostObservationResponse>();
            observation.Id = res.Id;
        }

        // generate 50 random comments in total (each comment is attached to one of the randomly generated observations)
        var comments = (
            from obs in Gen.Elements(observations)
            from author in string_gen
            from comment in string_gen
            from timestamp in date_gen
            select new CommentRecord()
            {
                ObservationId = obs.Id,
                Author = author.ToString(),
                Comment = comment.ToString(),
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(timestamp)
            }
        ).Sample(50);
        // save them
        foreach (var comment in comments)
        {
            var req = await DBClient.PostAsJsonAsync($"/observation/{comment.ObservationId}/comment", comment);
            req.EnsureSuccessStatusCode();
        }

        // generate 35 random proposals in total (each comment is attached to one of the randomly generated observations)
        var proposals = (
            from obs in Gen.Elements(observations)
            from author in string_gen
            from taxonId in TaxonIdGenerator().Generator
            from timestamp in date_gen
            select new ProposalRecord()
            {
                ObservationId = obs.Id,
                Author = author.ToString(),
                TaxonID = taxonId,
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(timestamp)
            }
        ).Sample(35);
        // save them
        foreach (var proposal in proposals)
        {
            var req = await DBClient.PostAsJsonAsync($"/observation/{proposal.ObservationId}/proposal", proposal);
            req.EnsureSuccessStatusCode();
        }

        // check if every generated observation exists
        foreach (var observation in observations)
        {
            var res = await DBClient.GetFromJsonAsync<ObservationRecord>($"/observation/{observation.Id}");
            res.Should().NotBeNull();
            res.Should().Be(observation);
        }

        // check if every generated comment exists for each observation
        var commentsForObservation = comments.GroupBy(v => v.ObservationId).ToDictionary(v => v.Key, k => k.ToList());
        foreach (var entry in commentsForObservation)
        {
            var coms = await DBClient.GetFromJsonAsync<List<CommentRecord>>($"/observation/{entry.Key}/comments");
            ScrambledEquals(entry.Value, coms).Should().BeTrue();
        }

        // check if every generated proposal exists for each observation
        var proposalsForObservation = proposals.GroupBy(v => v.ObservationId).ToDictionary(v => v.Key, k => k.ToList());
        foreach (var entry in proposalsForObservation)
        {
            var coms = await DBClient.GetFromJsonAsync<List<ProposalRecord>>($"/observation/{entry.Key}/proposals");
            ScrambledEquals(entry.Value, coms).Should().BeTrue();
        }
    }
}

internal class PostObservationResponse
{
    public int Id { get; set; }
}
