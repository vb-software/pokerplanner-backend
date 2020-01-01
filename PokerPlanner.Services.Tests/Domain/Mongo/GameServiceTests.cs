using System;
using System.Threading.Tasks;
using Moq;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;
using PokerPlanner.Repositories.Interfaces.Domain.Mongo;
using PokerPlanner.Services.Domain.Mongo;
using PokerPlanner.Services.Interfaces.Domain.Mongo;
using Xunit;

namespace PokerPlanner.Services.Tests.Domain.Mongo
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _gameRepository;
        private readonly Mock<IWorkspaceService> _workspaceService;
        private readonly GameService _gameService;

        public GameServiceTests()
        {
            _gameRepository = new Mock<IGameRepository>();
            _workspaceService = new Mock<IWorkspaceService>();

            _gameService = new GameService(_gameRepository.Object, _workspaceService.Object);
        }

        [Fact]
        public async Task CreatePublicGameTest()
        {
            var createGameDto = new CreateGameDto { IsPublic = true };
            var returnedGame = new Game();

            _gameRepository.Setup(repo => repo.CreateOrUpdateGame(It.IsAny<Game>())).ReturnsAsync(returnedGame);

            returnedGame = await _gameService.CreateGame(createGameDto);

            Assert.NotNull(returnedGame);
            Assert.IsType<Game>(returnedGame);
            Assert.True(returnedGame.IsPublic);
            Assert.NotEqual(DateTime.MinValue, returnedGame.CreatedOn);
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, false)]
        public async Task CreateGameWithinWorkspaceTest(bool configurationIsPublic, bool inputGameIsPublic, bool expectedGameIsPublic)
        {
            var createGameDto = new CreateGameDto
            {
                IsPublic = inputGameIsPublic,
                WorkspaceGuid = Guid.NewGuid(),
                ReleaseGuid = Guid.NewGuid(),
                IterationGuid = Guid.NewGuid()
            };

            var returnedGame = new Game();

            _gameRepository.Setup(repo => repo.CreateOrUpdateGame(It.IsAny<Game>())).ReturnsAsync(returnedGame);
            _workspaceService.Setup(service => service.GetIterationByGuid(It.IsAny<Guid>())).ReturnsAsync(new Iteration());
            _workspaceService.Setup(
                service =>
                service.GetWorkspaceById(It.IsAny<Guid>())).ReturnsAsync(
                new Workspace
                {
                    Configuration = new Configuration
                    {
                        IsPublic = configurationIsPublic
                    }
                });

            returnedGame = await _gameService.CreateGame(createGameDto);

            Assert.NotNull(returnedGame);
            Assert.IsType<Game>(returnedGame);
            Assert.Equal(expectedGameIsPublic, returnedGame.IsPublic);
            Assert.NotEqual(DateTime.MinValue, returnedGame.CreatedOn);
            Assert.NotNull(returnedGame.Iteration);
            Assert.NotNull(returnedGame.Iteration.UserStories);
            Assert.NotEqual(Guid.Empty, returnedGame.WorkspaceGuid);
            Assert.NotEqual(Guid.Empty, returnedGame.ReleaseGuid);
        }

        [Fact]
        public async Task SaveGameTest()
        {
            await _gameService.SaveGame(new Game());

            _gameRepository.Verify(repo => repo.CreateOrUpdateGame(It.IsAny<Game>()), Times.Once);
        }
    }
}