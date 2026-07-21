using System.ComponentModel.DataAnnotations;

namespace GameCollectionApi.DTO;

/// <summary>The fields to change on an existing game.</summary>
public sealed record PatchGameRequest(
    [StringLength(100)] string? Title,
    [StringLength(50)] string? Genre,
    [Range(1950, 2100)] ushort? ReleaseYear);
