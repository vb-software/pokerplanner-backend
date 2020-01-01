using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;
using PokerPlanner.Repositories.Interfaces.Domain.Mongo;
using PokerPlanner.Services.Interfaces.Domain.Mongo;

namespace PokerPlanner.Services.Domain.Mongo
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IWorkspaceService _workspaceService;

        public GameService(IGameRepository gameRepository, IWorkspaceService workspaceService)
        {
            _gameRepository = gameRepository;
            _workspaceService = workspaceService;
        }
        public async Task<Game> CreateGame(CreateGameDto createGameDto)
        {
            var game = new Game
            {
                Guid = Guid.NewGuid(),
                CreatedOn = DateTime.Now.ToUniversalTime(),
                IsPublic = createGameDto.IsPublic
            };

            if (createGameDto.IterationGuid != Guid.Empty)
            {
                var iteration = await _workspaceService.GetIterationByGuid(createGameDto.IterationGuid);

                if (iteration != null)
                {
                    game.Iteration = iteration;
                    game.Iteration.UserStories = game.Iteration.UserStories == null ? new List<UserStory>() : game.Iteration.UserStories;
                }

                if (createGameDto.WorkspaceGuid != Guid.Empty)
                {
                    var workspace = await _workspaceService.GetWorkspaceById(createGameDto.WorkspaceGuid);
                    game.IsPublic = !workspace.Configuration.IsPublic ? workspace.Configuration.IsPublic : createGameDto.IsPublic;
                    game.WorkspaceGuid = createGameDto.WorkspaceGuid;
                }

                game.ReleaseGuid = createGameDto.ReleaseGuid;
            }

            await _gameRepository.CreateOrUpdateGame(game);

            return game;
        }

        public async Task SaveGame(Game game)
        {
            await _gameRepository.CreateOrUpdateGame(game);
        }
    }
}