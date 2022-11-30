namespace Eclipse.Model
{
    public class EclipseGame
    {
        public Guid Id { get; set; }
        public string GameName = "Eclipse";
        public DateTime DateOfGame { get; set; }
        public string? Town { get; set; }
        public int? WinningScore { get; set; }

    }
}