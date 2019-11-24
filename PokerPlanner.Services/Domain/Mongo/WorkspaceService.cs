using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;
using PokerPlanner.Repositories.Interfaces.Domain.Mongo;
using PokerPlanner.Services.Extensions;
using PokerPlanner.Services.Interfaces.Domain.Mongo;

namespace PokerPlanner.Services.Domain.Mongo
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IUserRepository _userRepo;
        private readonly IWorkspaceRepository _workspaceRepo;
        private readonly IMapper _mapper;

        public WorkspaceService(IUserRepository userRepo, IWorkspaceRepository workspaceRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _workspaceRepo = workspaceRepo;
            _mapper = mapper;
        }

        public async Task<Workspace> AddReleaseToWorkspace(Guid workspaceId, CreateWorkspaceReleaseDto releaseDto)
        {
            var workspaceFromRepo = await _workspaceRepo.GetWorkspaceById(workspaceId);

            if (workspaceFromRepo == null)
            {
                return null;
            }

            var newWorkspaceRelease = _mapper.Map<Release>(releaseDto);

            newWorkspaceRelease.Guid = Guid.NewGuid();

            if (workspaceFromRepo.Releases.IsNullOrEmpty())
            {
                workspaceFromRepo.Releases = new List<Release>();
            }

            workspaceFromRepo.Releases.Add(newWorkspaceRelease);

            await _workspaceRepo.CreateOrUpdateWorkspace(workspaceFromRepo);

            return workspaceFromRepo;
        }

        public async Task<Workspace> CreateWorkspaceForUser(string username, CreateWorkspaceDto workspaceDto)
        {
            var userExists = await _userRepo.UserExists(username);

            if (!userExists)
            {
                return null;
            }

            var user = await _userRepo.GetUserByUsername(username);

            var newWorkspace = _mapper.Map<Workspace>(workspaceDto);

            newWorkspace.Guid = Guid.NewGuid();
            newWorkspace.OwnerGuid = user.Guid;

            await _workspaceRepo.CreateOrUpdateWorkspace(newWorkspace);

            return newWorkspace;
        }

        public async Task<Workspace> GetWorkspaceByUserAndId(string username, Guid workspaceGuid)
        {
            var userExists = await _userRepo.UserExists(username);

            if (!userExists)
            {
                return null;
            }

            var user = await _userRepo.GetUserByUsername(username);

            var workspacesByUser = await _workspaceRepo.GetWorkspacesByUser(user.Guid);

            if (workspacesByUser == null)
            {
                return null;
            }

            return workspacesByUser.FirstOrDefault(x => x.Guid == workspaceGuid);
        }

        public async Task<List<Workspace>> GetWorkspacesByUser(string username)
        {
            var userExists = await _userRepo.UserExists(username);

            if (!userExists)
            {
                return null;
            }

            var user = await _userRepo.GetUserByUsername(username);

            var workspacesByUser = await _workspaceRepo.GetWorkspacesByUser(user.Guid);

            return workspacesByUser;
        }
    }
}