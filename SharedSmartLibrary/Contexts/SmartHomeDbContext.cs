using Microsoft.EntityFrameworkCore;
using SharedSmartLibrary.Entities;

namespace SharedSmartLibrary.Contexts;

public class SmartHomeDbContext : DbContext
{
    public SmartHomeDbContext()
    {
        
    }
    public SmartHomeDbContext(DbContextOptions<SmartHomeDbContext> options) : base(options)
    {
        Database.EnsureCreated();
        try
        {
            Database.Migrate();
        }
        catch{ }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite();
    }

    public DbSet<SmartAppSettings> Settings { get; set; }
}
