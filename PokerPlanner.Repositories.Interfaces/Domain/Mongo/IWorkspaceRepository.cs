using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokerPlanner.Entities.Domain.Mongo;

namespace PokerPlanner.Repositories.Interfaces.Domain.Mongo
{
    public interface IWorkspaceRepository : IRepository
    {
         Task<Workspace> CreateWorkspace(Workspace workspace);
         Task<List<Workspace>> GetWorkspaces();
         Task<List<Workspace>> GetWorkspacesByUser(Guid userGuid);
    }
}