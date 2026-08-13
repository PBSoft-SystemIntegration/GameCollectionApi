using GameCollectionApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameCollectionApi.Data;

public sealed class GameDbContext(DbContextOptions<GameDbContext> options)
    : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<ApiClient> ApiClients { get; set; }
    public DbSet<User> Users => Set<User>();

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
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Username).HasMaxLength(100).IsRequired();
            entity.Property(user => user.PasswordHash).IsRequired();
            entity.Property(user => user.Role).HasMaxLength(50).IsRequired();
            entity.Property(user => user.RefreshTokenHash).HasMaxLength(64);
            entity.HasIndex(user => user.Username).IsUnique();
            entity.HasIndex(user => user.RefreshTokenHash).IsUnique();
        });
    }
}
