public class EclipseReportModel
{



    public Guid Id { get; set; }
    public string? Town { get; set; }
    public TimeSpan? TimeInterval { get; set; }
    public int GamesPlayed { get; set; }
    public int Highscore { get; set; }
    public string? UsernameHighscore { get; set; }

}