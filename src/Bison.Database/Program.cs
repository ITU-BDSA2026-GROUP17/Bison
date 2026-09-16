using Bison.Database;
using Bison.Models;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var csvFiles = config.GetSection("CSVFiles").Get<CSVFiles>();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDatabaseService, CSVDatabase>((v) => new CSVDatabase(csvFiles.Observations, csvFiles.Comments, csvFiles.ObservationIds));

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
        dbService.StoreObservation(obs);
        return Results.Created();
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


app.Run();

public class CSVFiles
{
    public required string Observations { get; set; }
    public required string Comments { get; set; }
    public required string ObservationIds { get; set; }
}
public partial class Program { }