using GameCollectionApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameCollectionApi.Data;

public sealed class GameDbContext(DbContextOptions<GameDbContext> options)
    : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<ApiClient> ApiClients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("games");
            entity.HasKey(game => game.Id);
            entity.Property(game => game.Title).HasMaxLength(100).IsRequired();
            entity.Property(game => game.Genre).HasMaxLength(50).IsRequired();
            entity.Property(game => game.ReleaseYear).HasConversion<int>();
        });
        modelBuilder.Entity<ApiClient>(entity =>
        {
            entity.ToTable("api_clients");
            entity.HasKey(client => client.Id);
            entity.Property(client => client.Name).HasMaxLength(100).IsRequired();
            entity.Property(client => client.ApiKeyHash).HasMaxLength(64).IsRequired();
            entity.HasIndex(client => client.ApiKeyHash).IsUnique();
        });
    }
}
