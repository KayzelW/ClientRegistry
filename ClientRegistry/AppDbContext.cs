using Microsoft.EntityFrameworkCore;

namespace ClientRegistry.Models;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbSet<Founder> Founders { get; set; }
    public DbSet<Client> Clients { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
#endif
        if (optionsBuilder.IsConfigured) return;
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("postgres"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Founder>()
            .HasMany(f => f.Clients)
            .WithMany(c => c.Founders);
    }
}