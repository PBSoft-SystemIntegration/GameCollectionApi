using GameCollectionApi.Models;
using GameCollectionApi.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;

namespace GameCollectionApi.Services;

public sealed class AuthService(
    IUserRepository userRepository,
    IConfiguration configuration) : IAuthService
{
    public async Task<AuthTokens?> RegisterAsync(string username, string password)
    {
        if (await userRepository.GetByUsernameAsync(username) is not null)
        {
            return null;
        }

        var user = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "User"
        };

        await userRepository.CreateAsync(user);
        return await GenerateAndSaveTokensAsync(user);
    }

    public async Task<AuthTokens?> LoginAsync(string username, string password)
    {
        var user = await userRepository.GetByUsernameAsync(username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }

        return await GenerateAndSaveTokensAsync(user);
    }

    public async Task<AuthTokens?> RefreshAsync(string refreshToken)
    {
        string refreshTokenHash = HashRefreshToken(refreshToken);
        var user = await userRepository.GetByRefreshTokenHashAsync(refreshTokenHash);

        if (user is null ||
            user.RefreshTokenExpiryTimeUtc is null ||
            user.RefreshTokenExpiryTimeUtc <= DateTime.UtcNow)
        {
            return null;
        }

        return await GenerateAndSaveTokensAsync(user);
    }

    public async Task SeedAdminAsync()
    {
        if (await userRepository.GetByUsernameAsync("admin") is not null)
        {
            return;
        }

        await userRepository.CreateAsync(new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = "Admin"
        });
    }

    private string GenerateJwt(User user)
    {
        string secret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret is missing.");

        Claim[] claims =
        [
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role)
        ];

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<AuthTokens> GenerateAndSaveTokensAsync(User user)
    {
        string refreshToken = GenerateRefreshToken();
        user.RefreshTokenHash = HashRefreshToken(refreshToken);
        user.RefreshTokenExpiryTimeUtc = DateTime.UtcNow.AddDays(7);
        await userRepository.UpdateAsync(user);

        return new AuthTokens(
            GenerateJwt(user),
            refreshToken);
    }

    private static string GenerateRefreshToken()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }

    private static string HashRefreshToken(string refreshToken)
    {
        byte[] hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(hash);
    }
}
