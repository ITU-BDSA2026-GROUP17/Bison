using Bison.Database;
using Bison.Models;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var csvFiles = config.GetSection("CSVFiles").Get<CSVFiles>();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDatabaseService, CSVDatabase>((v) => new CSVDatabase(csvFiles.Observations, csvFiles.Comments, csvFiles.ObservationIds, csvFiles.Proposals));

var app = builder.Build();

app.MapGet("/", () => "Hello World");

app.MapGet("/observations", (IDatabaseService dbService) => dbService.ReadObservations().ToArray());
app.MapGet("/observation/{id}", (int id, IDatabaseService dbService) =>
{
    foreach (var observation in dbService.ReadObservations())
    {
        if (observation.Id == id)
        {
            return Results.Json(observation);
        }
    }
    return Results.NotFound();
});
app.MapPost("/observation", (ObservationRecord obs, IDatabaseService dbService) =>
{
    try
    {
        var id = dbService.StoreObservation(obs);
        return Results.Created($"/observation/{id}", new { Id = id });
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.Problem("Could not store observation");
    }
});

app.MapGet("/observation/{id}/comments", (int id, IDatabaseService dbService) => dbService.ReadCommentsForObservation(id).ToArray());
app.MapPost("/observation/{id}/comment", (int id, CommentRecord comment, IDatabaseService dbService) =>
{
    try
    {
        comment.ObservationId = id;
        dbService.StoreComment(comment);
        return Results.Created();
    }
    catch (ObservationDoesNotExist e)
    {
        return Results.BadRequest(e.Message);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.Problem("Could not store comment");
    }
});

app.MapGet("/observation/{id}/proposals", (int id, IDatabaseService dbService) => dbService.ReadProposalsForObservation(id).ToArray());
app.MapPost("/observation/{id}/proposal", (int id, ProposalRecord proposal, IDatabaseService dbService) =>
{
    try
    {
        proposal.ObservationId = id;
        dbService.StoreProposal(proposal);
        return Results.Created();
    }
    catch (ObservationDoesNotExist e)
    {
        return Results.BadRequest(e.Message);
    }
    catch (TaxonDoesNotExist e)
    {
        return Results.BadRequest(e.Message);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.Problem("Could not store proposal");
    }
});

app.Run();

public class CSVFiles
{
    public required string Observations { get; set; }
    public required string Comments { get; set; }
    public required string ObservationIds { get; set; }
    public required string Proposals { get; set; }
}
public partial class Program { }
