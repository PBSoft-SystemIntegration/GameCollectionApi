using System.ComponentModel.DataAnnotations;

namespace GameCollectionApi.DTO;

/// <summary>The complete replacement data for a game.</summary>
public record UpdateGameRequest(
    [Required, StringLength(100)] string Title,
    [Required, StringLength(50)] string Genre,
    [Range(1950, 2100)] ushort ReleaseYear);
