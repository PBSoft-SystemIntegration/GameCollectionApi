namespace GameCollectionApi.Models;

public sealed class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public required string Role { get; set; }
    public string? RefreshTokenHash { get; set; }
    public DateTime? RefreshTokenExpiryTimeUtc { get; set; }
}
