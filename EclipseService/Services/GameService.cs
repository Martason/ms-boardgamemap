using Grpc.Core;
using EclipseService.Protos;
using Eclipse.Model;

namespace EclipseService.Services
{
    public class GameService : Games.GamesBase
    {
        private readonly ILogger<GameService> _logger;
        private readonly EclipseDbContext _context;

        public GameService(ILogger<GameService> logger, EclipseDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public override async Task<GameResponseModel> GetGames(GameRequestModel request, ServerCallContext context)
        {
            var response = new GameResponseModel();
            var games = _context.EclipseGames.ToList();

            if (games != null)
            {
                foreach (var game in games)
                {
                    response.Games.Add(new GameModel
                    {
                        Id = game.Id.ToString(),
                        GameName = game.GameName,
                        //DateOfGame = game.DateOfGame,
                        Town = game.Town,
                        WinningScore = game.WinningScore,
                        Username = game.UserName,
                    });
                }
            }

            return response;
        }



    }
}