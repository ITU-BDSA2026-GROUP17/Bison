using Bison.Models;

using Microsoft.EntityFrameworkCore;

namespace Bison.Database;

public class BisonDBContext : DbContext
{
    public DbSet<Observation> Obserations { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(c => c.Name)
            .IsUnique();
    }
}