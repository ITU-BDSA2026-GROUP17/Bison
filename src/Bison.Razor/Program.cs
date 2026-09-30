using Bison.Database;
using Bison.Database.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<BisonDbContext>(options =>
{
    if (Environment.GetEnvironmentVariable("BISONDBPATH") is string dbPath)
    {
        options.UseSqlite($"Data Source={dbPath}");
    } else
    {
        var tempPath = Path.Join(Path.GetTempPath(), "bison.db");
        options.UseSqlite($"Data Source={tempPath}");
    }
});
builder.Services.AddScoped<IDatabaseRepository, DatabaseRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IObservationService, ObservationService>();
builder.Services.AddScoped<IProposalService, ProposalService>();
builder.Services.AddScoped<ICommentService, CommentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();
