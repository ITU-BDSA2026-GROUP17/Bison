using Bison.Database;
using Bison.Models;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var csvFiles = config.GetSection("CSVFiles").Get<CSVFiles>();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDatabaseService, CSVDatabase>((v) => new CSVDatabase(csvFiles.Observations, csvFiles.Comments, csvFiles.ObservationIds));

var app = builder.Build();

app.MapGet("/", () => "Hello World");

app.Run();

public class CSVFiles
{
    public required string Observations{get; set;}
    public required string Comments{get; set;}
    public required string ObservationIds{get; set;}
}