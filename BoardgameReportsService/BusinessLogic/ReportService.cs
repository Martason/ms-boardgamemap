namespace BoardgameReportsService.BusinessLogic
{
    public interface IReportService
    {
        ///<summary>
        /// Builds and returns boardgame reports
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
            var testendpoint = "https://pokeapi.co/api/v2/pokemon/ditto";

            var testdataNotInUse = await httpClient.GetAsync(testendpoint + "/" + town.ToLower());
            var testdata = testEclipseData.Where(data => data.Town == town).ToList();
            return testdata;

        }
        private async Task<List<MonopolyModel>> FetchMonopolyData(HttpClient httpClient, string town)
        {
            var testendpoint = "https://pokeapi.co/api/v2/pokemon?limit=100&offset=10";

            var testdataNotInUse = await httpClient.GetAsync(testendpoint + "/" + town.ToLower());
            var testdata = testMonopolyData.Where(data => data.Town == town).ToList();
            return testdata;
        }

        #region testData         
        List<EclipseModel> testEclipseData = new List<EclipseModel>
        {
            new()
            {
                DateOfGame = DateTime.Today,
                Town = "kalix",
                WinningScore = 42
            },
            new()
            {
                DateOfGame = new DateTime(2022,11,19),
                Town = "kalix",
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
                Town = "kalix",
                WinningScore = 402
            },
            new()
            {
                DateOfGame = new DateTime(2022,11,19),
                Town = "kalix",
                WinningScore = 512
            },
            new()
            {
                DateOfGame = new DateTime(2022,11,09),
                Town = "lerum",
                WinningScore = 306
            }
        };
        #endregion

    }
}