namespace Bison.Database.Tests;

using Bison.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private string _obsPath;
    private string _comPath;
    private string _obsIdPath;
    private string _proposalPath;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(IDatabaseService));

            services.Remove(dbConnectionDescriptor);

            _obsPath = Path.GetTempFileName();
            _comPath = Path.GetTempFileName();
            _obsIdPath = Path.GetTempFileName();
            _proposalPath = Path.GetTempFileName();

            services.AddSingleton<IDatabaseService, CSVDatabase>(container =>
                new(_obsPath, _comPath, _obsIdPath, _proposalPath)
            );
        });
    }

    protected override void Dispose(bool disposing)
    {
        SavingRetrievingTest.DeleteFileIfExists(_obsPath);
        SavingRetrievingTest.DeleteFileIfExists(_comPath);
        SavingRetrievingTest.DeleteFileIfExists(_obsIdPath);
        SavingRetrievingTest.DeleteFileIfExists(_proposalPath);

        base.Dispose(disposing);
    }
}

public class WebServiceTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly int _id;

    public WebServiceTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;

        var client = factory.CreateClient();
        var postTask = client.PostAsJsonAsync("/observation", new ObservationRecord()
        {
            Author = "Arthur",
            Observation = "Saw a little heron",
            Timestamp = 0,
            Location = "At the canal"
        });
        postTask.Wait();
        _id = postTask.Result.Content.As<int>();
        client.PostAsJsonAsync($"/observation/{_id}/comment", new CommentRecord()
        {
            Author = "Fredrick",
            Comment = "It's so cute!!",
            Timestamp = 100
        });
        client.PostAsJsonAsync($"/observation/{_id}/proposal", new ProposalRecord()
        {
            Author = "Fredrick",
            TaxonID = "MSTSNM:Arter:c18811f4-f785-ea11-aa77-501ac539d1ea",
            Timestamp = 200
        });
    }

    [Theory]
    [InlineData("/observations")]
    [InlineData("/observation/0")]
    [InlineData("/observation/0/comments")]
    [InlineData("/observation/0/proposals")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode(); // Status Code 200-299
        response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");
    }

    /*
    currently does not work, as there is an issue with CSV files being used by multiple processes
    -> this test should be enabled once having moved to SQLite

    [Fact]
    public async Task Post_EndpointsReturnSuccess()
    {
        var client = _factory.CreateClient();

        var res1 = await client.PostAsJsonAsync("/observation",
            new ObservationRecord()
            {
                Author = "Fredrick",
                Observation = "A heron!!",
                Location = "At the canal",
                Timestamp = 5000,
            }
        );
        res1.EnsureSuccessStatusCode(); // Status Code 200-299

        var res2 = await client.PostAsJsonAsync($"/observation/{_id}/comment",
             new CommentRecord()
             {
                 Author = "Arthur",
                 Comment = "I know right??",
                 Timestamp = 1000,
             }
        );
        res2.EnsureSuccessStatusCode(); // Status Code 200-299
    }
    */
}
