namespace GameCollectionApi.DTO
{
    public record GameResponse(
      int Id,
      string Title,
      string Genre,
      ushort ReleaseYear);

}
