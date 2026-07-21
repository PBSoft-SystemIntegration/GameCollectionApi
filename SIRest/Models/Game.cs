namespace GameCollectionApi.Models
{
    public class Game
    {
        public required string Title { get; set; }
        public required string Genre { get; set; }
        public ushort ReleaseYear { get; set; }
    }

}
