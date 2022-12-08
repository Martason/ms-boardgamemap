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

        public async Task<List<Game>> GetMonopolyGames()
        {
            var monopolyGames = await _httpClient.GetFromJsonAsync<List<MonopolyGame>>("/monopoly");
            if (monopolyGames is null) return null;
            var games = new List<Game>();
            foreach (var game in monopolyGames)
            {
                games.Add(new Game
                {
                    Name = "Monopoly",
                    DateOfGame = game.DateOfGame,
                    Town = game.Town,
                    WinningScore = game.WinningScore,
                    UserName = game.UserName
                });
            }
            return games;
        }

        public async Task<bool> PostMonopolyGame(GameInput input)
        {
            var monopolyGame = await _httpClient.PostAsJsonAsync<GameInput>($"/monopoly", input);
            return monopolyGame.IsSuccessStatusCode;
        }

    }
}