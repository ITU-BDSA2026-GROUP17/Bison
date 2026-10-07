namespace Bison.Database.Tests;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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

public class UnitTests(DbOptions dbOptions) : IClassFixture<DbOptions>
{
    private readonly DbOptions _dbOptions = dbOptions;
    private readonly DatabaseRepository _databaseRepository = new(dbOptions.BisonDbContext);
}
