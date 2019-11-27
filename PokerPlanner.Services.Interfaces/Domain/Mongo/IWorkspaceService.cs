using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;

namespace PokerPlanner.Services.Interfaces.Domain.Mongo {
    public interface IWorkspaceService : IService {
        Task<Workspace> CreateWorkspaceForUser(string username, CreateWorkspaceDto workspaceDto);
        Task<List<Workspace>> GetWorkspacesByUser(string username);
        Task<Workspace> GetWorkspaceByUserAndId(string username, Guid workspaceGuid);
        Task<Workspace> AddReleaseToWorkspace(Guid workspaceId, CreateWorkspaceReleaseDto releaseDto);
        Task<Release> AddIterationToWorkspaceRelease(Guid workspaceId, Guid releaseId, CreateWorkspaceReleaseIterationDto iterationDto);
        Task<List<WorkspaceSummaryDto>> GetWorkspaceSummariesByUser(string username);
        Task<Workspace> AddUserToWorkspace(Guid workspaceId, UserDto userDto);
    }
}