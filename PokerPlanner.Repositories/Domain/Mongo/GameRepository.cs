using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MongoDB.Driver;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.Settings;
using PokerPlanner.Repositories.Domain.Mongo.Extensions;
using PokerPlanner.Repositories.Interfaces.Domain.Mongo;
using PokerPlanner.Repositories.Mongo;

namespace PokerPlanner.Repositories.Domain.Mongo
{
    public class GameRepository : MongoRepositoryBase, IGameRepository
    {
        public GameRepository(MongoDbSettings mongoDbSettings)
        {
            _mongoDbSettings = mongoDbSettings;
        }
        private IMongoCollection<Game> GetCollection()
        {
            var collection = Db.GetGlobalCollection<Game>();

            return collection;
        }
        public async Task<Game> CreateOrUpdateGame(Game game)
        {
            if (game.Guid == Guid.Empty)
            {
                game.Guid = Guid.NewGuid();
            }

            var collection = GetCollection();

            await collection.ReplaceOneAsync(x => x.Guid == game.Guid, game, new UpdateOptions { IsUpsert = true });

            return game;
        }
    }
}