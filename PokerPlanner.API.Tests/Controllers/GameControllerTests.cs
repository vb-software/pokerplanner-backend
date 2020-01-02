using System.Net;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Moq;
using PokerPlanner.API.Controllers;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;
using PokerPlanner.Services.Interfaces.Domain.Mongo;
using Xunit;

namespace PokerPlanner.API.Tests.Controllers
{
    public class GameControllerTests
    {
        private readonly Mock<IGameService> _gameService;
        private readonly GameController _controller;

        public GameControllerTests()
        {
            _gameService = new Mock<IGameService>();
            _controller = new GameController(_gameService.Object);
        }

        [Fact]
        public async Task CreateGameTest()
        {
            var game = new Game();

            _gameService.Setup(service => service.CreateGame(It.IsAny<CreateGameDto>())).ReturnsAsync(game);

            var returnedGame = await _controller.CreateGame(new CreateGameDto());

            Assert.NotNull(game);
            Assert.Equal(game, returnedGame);
        }

        [Fact]
        public async Task SaveGameTest()
        {
            var game = new Game();

            var response = await _controller.SaveGame(game);

            _gameService.Verify(service => service.SaveGame(game), Times.Once);

            Assert.NotNull(response);
            Assert.IsType<ApiResponse>(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        }
    }
}