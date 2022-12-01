namespace BoardgameReportsService.BusinessLogic
{
    ///<summary>
    /// Sammanställer data från olika extrena tjänster och bygger brädspelsrapporter
    ///</summary>
    public interface IReportService
    {
        ///<summary>
        /// Builds and returns boardgame reports  data från olika extrena tjänster och bygger brädspelsrapporter
        ///</summary>
        public Task<TownReportModel> BuildBoardgameReport(string town);
    }

    public class ReportService : IReportService
    {

        private readonly IHttpClientFactory _http;

        public ReportService(IHttpClientFactory http)
        {
            _http = http;
        }

        public async Task<TownReportModel> BuildBoardgameReport(string town)
        {
            var httpClient = _http.CreateClient();

            var eclipseData = await FetchEclipseData(httpClient, town);
            var monopolyData = await FetchMonopolyData(httpClient, town);

            var newReport = new TownReportModel()
            {
                Town = town,
                GamesEcplisePlayed = eclipseData.Count,
                HighscoreEclipse = eclipseData.Max(data => data.WinningScore),
                GamesMonopolyPlayed = monopolyData.Count,
                HighscoreMonopoly = monopolyData.Max(data => data.WinningScore),
                GamesPlayed = eclipseData.Count + monopolyData.Count
            };

            return newReport;
        }

        private async Task<List<EclipseModel>> FetchEclipseData(HttpClient httpClient, string town)
        {
            var endpoint = "";
            if (town != null)
            {
                endpoint = "";
            }
            else
            {
                endpoint = "";
            }
            var data = await httpClient.GetAsync(endpoint);
            return testEclipseData;

        }
        private async Task<List<MonopolyModel>> FetchMonopolyData(HttpClient httpClient, string town)
        {
            var endpoint = "";
            if (town != null)
            {
                endpoint = "";
            }
            else
            {
                endpoint = "";
            }
            var data = await httpClient.GetAsync(endpoint);
            return testMonopolyData;
        }

        #region testData         
        List<EclipseModel> testEclipseData = new List<EclipseModel>
        {
            new()
            {
                DateOfGame = DateTime.Today,
                Town = "Kalix",
                WinningScore = 42
            },
            new()
            {
                DateOfGame = new DateTime(2022,11,19),
                Town = "Kalix",
                WinningScore = 52
            },
            new()
            {
                DateOfGame = new DateTime(2022,11,09),
                Town = "Lerum",
                WinningScore = 36
            }
        };
        List<MonopolyModel> testMonopolyData = new List<MonopolyModel>
        {
            new()
            {
                DateOfGame = DateTime.Today,
                Town = "Kalix",
                WinningScore = 42
            },
            new()
            {
                DateOfGame = new DateTime(2022,11,19),
                Town = "Kalix",
                WinningScore = 52
            },
            new()
            {
                DateOfGame = new DateTime(2022,11,09),
                Town = "Lerum",
                WinningScore = 36
            }
        };
        #endregion

    }
}