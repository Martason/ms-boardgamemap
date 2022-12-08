using BoardgameReportsService.Data;

namespace BoardgameReportsService
{
    class MonopolyClient
    {
        private HttpClient _httpClient;
        public MonopolyClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MonopolyGame>> GetMonopolyGames()
        {
            var monopolyGames = await _httpClient.GetFromJsonAsync<List<MonopolyGame>>("/monopoly");
            if (monopolyGames is null) return null;
            return monopolyGames;
        }

        public async Task<bool> PostMonopolyGame(GameInput input)
        {
            var monopolyGame = await _httpClient.PostAsJsonAsync<GameInput>($"/monopoly", input);
            return monopolyGame.IsSuccessStatusCode;
        }

    }
}