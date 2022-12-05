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
            /*             var monopolyData = await FetchMonopolyData(town);
             */
            var newReport = new TownReportModel()
            {
                /*             Town = town,
                               GamesEcplisePlayed = eclipseData.Count,
                               HighscoreEclipse = eclipseData.Max(data => data.WinningScore),
                                GamesMonopolyPlayed = monopolyData.Count,
                               HighscoreMonopoly = monopolyData.Max(data => data.WinningScore),
                               GamesPlayed = eclipseData.Count + monopolyData.Count
                */
            };

            return newReport;
        }

        private async Task<List<EclipseModel>> FetchEclipseData(string town)
        {

            _httpClient.BaseAddress = new Uri("http://localhost:3000");
            var request = await _httpClient.GetFromJsonAsync<EclipseModel>("/");/* ($"/{town}"); */


            return null;

        }
        private async Task<List<MonopolyModel>> FetchMonopolyData(string town)
        {
            // TODO not working
            _httpClient.BaseAddress = new Uri("");
            var testdataNotInUse = await _httpClient.GetFromJsonAsync<EclipseModel>("");


            return null;
        }



    }
}