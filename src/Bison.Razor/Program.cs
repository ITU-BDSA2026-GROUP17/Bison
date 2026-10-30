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
    }
    else
    {
        var tempPath = Path.Join(Path.GetTempPath(), "bison.db");
        options.UseSqlite($"Data Source={tempPath}");
    }
});
builder.Services.AddDefaultIdentity<Bison.Models.User>(options =>
    options.SignIn.RequireConfirmedAccount = true
).AddEntityFrameworkStores<BisonDbContext>();
builder.Services.AddScoped<IDatabaseRepository, DatabaseRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IObservationService, ObservationService>();
builder.Services.AddScoped<IProposalService, ProposalService>();
builder.Services.AddScoped<ICommentService, CommentService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetService<BisonDbContext>()!;

    context.Database.Migrate();
    DbSeeder.SeedDatabase(context);
}

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();
