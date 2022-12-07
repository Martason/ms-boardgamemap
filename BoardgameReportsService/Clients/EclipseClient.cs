using BoardgameReportsService.Data;

namespace BoardgameReportsService
{
    class EclipseClient
    {
        private HttpClient _httpClient;
        public EclipseClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EclipseGame>> GetEclipseGames()
        {
            var eclipseGames = await _httpClient.GetFromJsonAsync<List<EclipseGame>>("/eclipse");
            if (eclipseGames is null) return null;
            return eclipseGames;
        }
    }
}