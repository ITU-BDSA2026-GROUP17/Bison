using Bison.Database;
using Bison.Database.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

using var tempLoggerFactory = LoggerFactory.Create(logging =>
    {
        logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
        logging.AddConsole();
    });
var logger = tempLoggerFactory.CreateLogger<Program>();

// Add services to the container.
logger.LogTrace("Adding Razor Pages");
builder.Services.AddRazorPages();

logger.LogTrace("Adding DbContext");
builder.Services.AddDbContext<BisonDbContext>((options) =>
    {
        if (builder.Configuration["BISONDBPATH"] is string dbPath)
        {
            options.UseSqlite($"Data Source={dbPath}");
        }
        else
        {
            var tempPath = Path.Join(Path.GetTempPath(), "bison.db");
            logger.LogWarning("Using temporary database at {tempPath}", tempPath);
            options.UseSqlite($"Data Source={tempPath}");
        }
    });
logger.LogTrace("Adding Identity");
builder.Services.AddDefaultIdentity<Bison.Models.User>(options =>
        options.SignIn.RequireConfirmedAccount = true
    ).AddEntityFrameworkStores<BisonDbContext>();
logger.LogTrace("Adding Repository");
builder.Services.AddScoped<IDatabaseRepository, DatabaseRepository>();
logger.LogTrace("Adding Services");
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IObservationService, ObservationService>();
builder.Services.AddScoped<IProposalService, ProposalService>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddAuthentication()
    .AddGitHub(o =>
    {
        o.ClientId = builder.Configuration["authentication:github:clientId"] ?? "";
        o.ClientSecret = builder.Configuration["authentication:github:clientSecret"] ?? "";
        o.CallbackPath = "/signin-github";
    });

logger.LogDebug("Building WebApplication");
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    logger.LogTrace("Getting DbContext to migrate and seed");
    using var context = scope.ServiceProvider.GetService<BisonDbContext>()!;

    logger.LogTrace("Will migrate any remaining migrations");
    context.Database.Migrate();
    logger.LogTrace("Will seed database (if needed)");
    DbSeeder.SeedDatabase(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    logger.LogInformation("Production Environment Detected");
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

logger.LogTrace("Setting up HTTPS and static files");
app.UseHttpsRedirection();
app.UseStaticFiles();

logger.LogTrace("Setting up HTTP routing");
app.UseRouting();

logger.LogTrace("Setting up Authentication");
app.UseAuthentication();
app.UseAuthorization();

logger.LogTrace("Mapping Razor Pages");
app.MapRazorPages();
logger.LogTrace("Starting App");
app.Run();