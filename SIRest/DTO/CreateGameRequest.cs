using System.ComponentModel.DataAnnotations;

namespace GameCollectionApi.DTO;

/// <summary>The data required to create a game.</summary>
public sealed record CreateGameRequest(
    [Required, StringLength(100)] string Title,
    [Required, StringLength(50)] string Genre,
    [Range(1950, 2100)] ushort ReleaseYear);
