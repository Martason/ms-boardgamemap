namespace BoardgameReportsService.Data
{
    public class Game
    {
        public string? Name { get; set; }
        public DateTime DateOfGame { get; set; }
        public string? Town { get; set; }
        public int? WinningScore { get; set; }
        public string? UserName { get; set; }
    }
}