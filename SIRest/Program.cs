using GameCollectionApi.Data;
using GameCollectionApi.Repositories;
using GameCollectionApi.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var repositoryProvider = builder.Configuration["Repository:Provider"] ?? "InMemory";
switch (repositoryProvider.ToLowerInvariant())
{
    case "inmemory":
        builder.Services.AddSingleton<IGameRepository, InMemoryGameRepository>();
        break;
    case "sqlite":
        builder.Services.AddDbContext<GameDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
        builder.Services.AddScoped<IGameRepository, EfGameRepository>();
        break;
    case "postgres":
        builder.Services.AddDbContext<GameDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
        builder.Services.AddScoped<IGameRepository, EfGameRepository>();
        break;
    default:
        throw new InvalidOperationException(
            $"Unknown repository provider '{repositoryProvider}'. " +
            "Use InMemory, Sqlite or Postgres.");
}

builder.Services.AddScoped<IGameService, GameService>();

var app = builder.Build();

if (!repositoryProvider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
