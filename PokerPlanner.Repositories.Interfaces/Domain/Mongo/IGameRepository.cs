using System.Threading.Tasks;
using PokerPlanner.Entities.Domain.Mongo;

namespace PokerPlanner.Repositories.Interfaces.Domain.Mongo
{
    public interface IGameRepository : IRepository
    { 
         Task<Game> CreateOrUpdateGame(Game game);
    }
}