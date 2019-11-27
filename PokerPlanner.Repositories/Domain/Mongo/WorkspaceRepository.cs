using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}