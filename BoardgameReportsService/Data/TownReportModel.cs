public class TownReportModel
{
    public Guid Id { get; set; }
    public string Town { get; set; }
    public TimeSpan? TimeInterval { get; set; }
    public int GamesEcplisePlayed { get; set; }
    public int? HighscoreEclipse { get; set; }

    public int GamesMonopolyPlayed { get; set; }
    public int? HighscoreMonopoly { get; set; }
    public int GamesPlayed { get; set; }
}