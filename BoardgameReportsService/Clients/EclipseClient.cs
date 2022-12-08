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

        public async Task<List<Game>> GetEclipseGames()
        {
            var eclipseGames = await _httpClient.GetFromJsonAsync<List<EclipseGame>>("/eclipse");
            if (eclipseGames is null) return null;

            var games = new List<Game>();
            foreach (var game in eclipseGames)
            {
                games.Add(new Game
                {
                    Name = "Eclipse",
                    DateOfGame = game.DateOfGame,
                    Town = game.Town,
                    WinningScore = game.WinningScore,
                    UserName = game.UserName
                });
            }
            return games;
        }

        public async Task<bool> PostEclipseGame(GameInput input)
        {
            var eclipseGame = await _httpClient.PostAsJsonAsync<GameInput>($"/eclipse", input);
            return eclipseGame.IsSuccessStatusCode;
        }

    }
}