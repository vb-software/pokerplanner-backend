using System.Threading.Tasks;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;

namespace PokerPlanner.Services.Interfaces.Domain.Mongo
{
    public interface IGameService : IService
    {
         Task<Game> CreateGame(CreateGameDto createGameDto);
         Task SaveGame(Game game);
    }
}