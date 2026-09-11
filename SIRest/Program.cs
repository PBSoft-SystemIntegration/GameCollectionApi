using GameCollectionApi.Attributes;
using GameCollectionApi.Data;
using GameCollectionApi.Repositories;
using GameCollectionApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

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
        break;
    case "sqlite":
        builder.Services.AddDbContext<GameDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
        builder.Services.AddScoped<IGameRepository, EfGameRepository>();
        builder.Services.AddScoped<IApiClientRepository, EfApiClientRepository>();
        break;
    case "postgres":
        builder.Services.AddDbContext<GameDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
        builder.Services.AddScoped<IGameRepository, EfGameRepository>();
        builder.Services.AddScoped<IApiClientRepository, EfApiClientRepository>();
        break;
    default:
        throw new InvalidOperationException(
            $"Unknown repository provider '{repositoryProvider}'. " +
            "Use InMemory, Sqlite or Postgres.");
}

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

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

                return Task.CompletedTask;
            });

    // Markerer endpoints med [ApiKey] som beskyttede
    options.AddOperationTransformer(
        (operation, context, cancellationToken) =>
        {
            bool requiresApiKey =
                context.Description.ActionDescriptor.EndpointMetadata
                    .OfType<ApiKeyAttribute>()
                    .Any();

            if (!requiresApiKey)
            {
                return Task.CompletedTask;
            }

            operation.Security ??= [];

            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(
                        "ApiKey",
                        context.Document!)] = []
                });

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("ApiDocumentation:Enabled"))
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.AddPreferredSecuritySchemes("ApiKey");
    });
}

if (!app.Configuration.GetValue<bool>("DOTNET_RUNNING_IN_CONTAINER"))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
