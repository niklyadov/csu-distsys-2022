using Microsoft.EntityFrameworkCore;

namespace TestApp.DAL;

public sealed class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions options) : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    public DbSet<LinkRecord> Links { get; set; } = default!;
}