using GameCollectionApi.DTO;
using GameCollectionApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameCollectionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterUserRequest request)
    {
        AuthTokens? tokens = await authService.RegisterAsync(request.Username, request.Password);
        return tokens is null
            ? Conflict("Username already exists.")
            : Ok(ToResponse(tokens));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        AuthTokens? tokens = await authService.LoginAsync(request.Username, request.Password);
        return tokens is null
            ? Unauthorized("Invalid username or password.")
            : Ok(ToResponse(tokens));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenRequest request)
    {
        AuthTokens? tokens = await authService.RefreshAsync(request.RefreshToken);
        return tokens is null
            ? Unauthorized("Invalid or expired refresh token.")
            : Ok(ToResponse(tokens));
    }

    private static AuthResponse ToResponse(AuthTokens tokens) =>
        new(tokens.Token, tokens.RefreshToken);
}
