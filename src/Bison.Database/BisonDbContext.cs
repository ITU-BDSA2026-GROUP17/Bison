using Bison.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bison.Database;

public class BisonDbContext(DbContextOptions<BisonDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Taxon> Taxons { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Taxon>()
            .HasIndex(c => c.DWC_TaxonId)
            .IsUnique();

        modelBuilder.Entity<Observation>();
        modelBuilder.Entity<Comment>();
        modelBuilder.Entity<Proposal>();
        modelBuilder.Entity<Post>()
            .Property(pro => pro.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<Post>()
            .Navigation(pro => pro.Author)
            .AutoInclude();
    }
}

internal class BisonContextFactory : IDesignTimeDbContextFactory<BisonDbContext>
{
    public BisonDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BisonDbContext>();
        optionsBuilder.UseSqlite($"Data Source=:memory:");

        return new BisonDbContext(optionsBuilder.Options);
    }
}
