namespace Monopoly.Model
{
    public class MonopolyGame
    {

        public Guid Id { get; set; }
        public string GameName = "Monopoly";
        public DateTime DateOfGame { get; set; }
        public string? Town { get; set; }
        public int? WinningScore { get; set; }
        public string? UserId { get; set; }

    }
}