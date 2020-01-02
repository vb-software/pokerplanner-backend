using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.Settings;
using PokerPlanner.Repositories.Domain.Mongo.Extensions;
using PokerPlanner.Repositories.Interfaces.Domain.Mongo;
using PokerPlanner.Repositories.Mongo;

namespace PokerPlanner.Repositories.Domain.Mongo
{
    public class WorkspaceRepository : MongoRepositoryBase, IWorkspaceRepository
    {
        public WorkspaceRepository(MongoDbSettings mongoDbSettings)
        {
            _mongoDbSettings = mongoDbSettings;
        }

        private IMongoCollection<Workspace> GetCollection()
        {
            var collection = Db.GetGlobalCollection<Workspace>();
            return collection;
        }

        public async Task<Workspace> CreateOrUpdateWorkspace(Workspace workspace)
        {
            var collection = GetCollection();

            await collection.ReplaceOneAsync(x => x.Guid == workspace.Guid, workspace, new UpdateOptions { IsUpsert = true });

            return workspace;
        }

        public async Task<List<Workspace>> GetWorkspaces()
        {
            var collection = GetCollection();

            var workspaces = await collection.Find(x => true).ToListAsync();

            return workspaces;
        }

        public async Task<List<Workspace>> GetWorkspacesByUser(Guid userGuid)
        {
            var collection = GetCollection();

            var workspaces = await collection.Find(x => x.OwnerGuid == userGuid).ToListAsync();

            return workspaces;
        }

        public async Task<Workspace> GetWorkspaceById(Guid workspaceId)
        {
            var collection = GetCollection();

            var workspace = await collection.Find(x => x.Guid == workspaceId).FirstOrDefaultAsync();

            return workspace;
        }

        // public async Task<UserStory> UpdateActiveStatusOnUserStory(
        //     Guid workspaceId,
        //     Guid releaseId,
        //     Guid iterationId,
        //     Guid userStoryId,
        //     bool active)
        // {
        //     var collection = GetCollection();
        //     var updateDefinition = Builders<Workspace>.Update.Set("releases.$[r].iterations.$[i].userStories.$[u].active", active);
        //     var arrayFilters = new List<ArrayFilterDefinition>
        //     {
        //         new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("r.guid", releaseId)),
        //         new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("i.guid", iterationId)),
        //         new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("u.guid", userStoryId))
        //     };
        //     var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };

        //     await collection.UpdateOneAsync(x => x.Guid == workspaceId, updateDefinition, updateOptions);

        //     var workspace = await collection.Find(x => x.Guid == workspaceId).FirstOrDefaultAsync();

        //     return workspace.Releases
        //         .First(x => x.Guid == releaseId).Iterations
        //         .First(x => x.Guid == iterationId).UserStories
        //         .First(x => x.Guid == userStoryId);
        // }
    }
}