using System.Net;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;
using PokerPlanner.Services.Interfaces.Domain.Mongo;

namespace PokerPlanner.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost]
        public async Task<Game> CreateGame(CreateGameDto createGameDto)
        {
            var game = await _gameService.CreateGame(createGameDto);

            return game;
        }

        [HttpPut]
        public async Task<ApiResponse> SaveGame(Game game)
        {
            await _gameService.SaveGame(game);

            return new ApiResponse("Game saved successfully", null, (int)HttpStatusCode.OK);
        }
    }
}