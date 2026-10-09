using Bison.Models;

using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace Bison.Database;

public class BisonDbContext(DbContextOptions<BisonDbContext> options) : DbContext(options)
{
    public DbSet<Observation> Observations { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Observation>()
            .Property(obs => obs.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Comment>()
            .Property(com => com.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Proposal>()
            .Property(pro => pro.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
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
