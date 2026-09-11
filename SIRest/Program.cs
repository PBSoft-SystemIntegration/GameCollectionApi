using GameCollectionApi.Attributes;
using GameCollectionApi.Data;
using GameCollectionApi.Repositories;
using GameCollectionApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var repositoryProvider = builder.Configuration["Repository:Provider"] ?? "InMemory";
switch (repositoryProvider.ToLowerInvariant())
{
    case "inmemory":
        builder.Services.AddSingleton<IGameRepository, InMemoryGameRepository>();
        builder.Services.AddSingleton<IApiClientRepository, InMemoryApiClientRepository>();
        builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        break;
    case "sqlite":
        builder.Services.AddDbContext<GameDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
        builder.Services.AddScoped<IGameRepository, EfGameRepository>();
        builder.Services.AddScoped<IApiClientRepository, EfApiClientRepository>();
        builder.Services.AddScoped<IUserRepository, EfUserRepository>();
        break;
    case "postgres":
        builder.Services.AddDbContext<GameDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
        builder.Services.AddScoped<IGameRepository, EfGameRepository>();
        builder.Services.AddScoped<IApiClientRepository, EfApiClientRepository>();
        builder.Services.AddScoped<IUserRepository, EfUserRepository>();
        break;
    default:
        throw new InvalidOperationException(
            $"Unknown repository provider '{repositoryProvider}'. " +
            "Use InMemory, Sqlite or Postgres.");
}

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]
                    ?? throw new InvalidOperationException("Jwt:Secret is missing.")))
        };
    });

builder.Services.AddOpenApi(options =>
{
        // Beskriver API-key-mekanismen i OpenAPI-dokumentet
        options.AddDocumentTransformer(
            (document, context, cancellationToken) =>
            {
                document.Components ??= new OpenApiComponents();

                document.Components.SecuritySchemes ??=
                    new Dictionary<string, IOpenApiSecurityScheme>();

                document.Components.SecuritySchemes["ApiKey"] =
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        Name = "x-api-key",
                        In = ParameterLocation.Header,
                        Description =
                            "Add API key. It will be added in the x-api-key-header."
                    };

                document.Components.SecuritySchemes["Bearer"] =
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Enter the JWT returned by the login endpoint."
                    };

                return Task.CompletedTask;
            });

    // Markerer endpoints med [ApiKey] som beskyttede
    options.AddOperationTransformer(
        (operation, context, cancellationToken) =>
        {
            var metadata = context.Description.ActionDescriptor.EndpointMetadata;
            bool requiresApiKey = metadata.OfType<ApiKeyAttribute>().Any();
            bool requiresJwt =
                metadata.OfType<IAuthorizeData>().Any() &&
                !metadata.OfType<IAllowAnonymous>().Any();

            if (!requiresApiKey && !requiresJwt)
            {
                return Task.CompletedTask;
            }

            operation.Security ??= [];

            if (requiresApiKey)
            {
                operation.Security.Add(
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(
                            "ApiKey",
                            context.Document!)] = []
                    });
            }

            if (requiresJwt)
            {
                operation.Security.Add(
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(
                            "Bearer",
                            context.Document!)] = []
                    });
            }

            return Task.CompletedTask;
        });
});

var app = builder.Build();

if (!repositoryProvider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
    await authService.SeedAdminAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("ApiDocumentation:Enabled"))
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.AddPreferredSecuritySchemes("ApiKey", "Bearer");
    });
}

if (!app.Configuration.GetValue<bool>("DOTNET_RUNNING_IN_CONTAINER"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
