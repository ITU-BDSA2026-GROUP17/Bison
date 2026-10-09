namespace Bison.Database.Tests;

using Microsoft.EntityFrameworkCore;

public class DbOptions : IDisposable
{
    public BisonDbContext BisonDbContext { get; init; }

    public DbOptions()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<BisonDbContext>();
        dbContextOptionsBuilder.UseSqlite("Data Source=:memory:");
        BisonDbContext = new(dbContextOptionsBuilder.Options);
        BisonDbContext.Database.OpenConnection();
        BisonDbContext.Database.Migrate();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        BisonDbContext.Database.CloseConnection();
        BisonDbContext.Dispose();
    }
}
