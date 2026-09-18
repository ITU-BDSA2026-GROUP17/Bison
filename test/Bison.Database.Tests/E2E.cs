namespace Bison.Database.Tests;

using System.Diagnostics;
using System.Net.Http.Headers;

using Bison.Models;

using FsCheck;
using FsCheck.Fluent;

using Xunit.Sdk;

public class E2E
{
    private static readonly Taxonomies Taxonomies = new();
    public static Arbitrary<string> TaxonIdGenerator()
    {
        Gen<string> taxonGenerator = Gen.Choose(0, Taxonomies.TaxonomiesList.Count - 1)
        .Select(i => Taxonomies.TaxonomiesList[i].TaxonID);
        return taxonGenerator.ToArbitrary();
    }
    public static Process StartDatabase()
    {
        using var process = Process.Start(new ProcessStartInfo()
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


    public static void InitializeDBClient()
    {
        DBClient.DefaultRequestHeaders.Accept.Clear();
        DBClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        DBClient.BaseAddress = new(DATABASE_URI);
    }

    [Fact]
    public static async Task Something()
    {
        InitializeDBClient();
        var DB = StartDatabase();
        List<ObservationRecord> observations = [];
 
        var res = await DBClient.PostAsJsonAsync("/observation", new ObservationRecord
        {
            Author = Environment.UserName,
            Observation = ,
            Location = location,
            Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
        });

        DB.Close();
    }
}