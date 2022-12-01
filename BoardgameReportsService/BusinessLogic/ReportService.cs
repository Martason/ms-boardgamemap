using System.Text.Json;

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

        private readonly HttpClient _httpClient;

        public ReportService(HttpClient httpClient) =>
        _httpClient = httpClient;

        public async Task<TownReportModel> BuildBoardgameReport(string town)
        {
            var eclipseData = await FetchEclipseData(town);
            var monopolyData = await FetchMonopolyData(town);

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

        private async Task<List<EclipseModel>> FetchEclipseData(string town)
        {
            // TODO not working
            _httpClient.BaseAddress = new Uri("https://pokeapi.co/api/v2/pokemon");
            var testdataNotInUse = await _httpClient.GetFromJsonAsync<EclipseModel>("");

            var testdata = testEclipseData.Where(data => data.Town == town.ToLower()).ToList();
            return testdata;

        }
        private async Task<List<MonopolyModel>> FetchMonopolyData(string town)
        {
            // TODO not working
            _httpClient.BaseAddress = new Uri("https://pokeapi.co/api/v2/pokemon");
            var testdataNotInUse = await _httpClient.GetFromJsonAsync<EclipseModel>("");

            var testdata = testMonopolyData.Where(data => data.Town == town.ToLower()).ToList();
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