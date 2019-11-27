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

        public async Task<Release> AddIterationToWorkspaceRelease(Guid workspaceId, Guid releaseId, CreateWorkspaceReleaseIterationDto iterationDto)
        {
            var workspaceFromRepo = await _workspaceRepo.GetWorkspaceById(workspaceId);

            if (workspaceFromRepo == null)
            {
                return null;
            }

            var newWorkspaceReleaseIteration = _mapper.Map<Iteration>(iterationDto);

            newWorkspaceReleaseIteration.Guid = Guid.NewGuid();

            var releaseToAddIteration = workspaceFromRepo.Releases.FirstOrDefault(x => x.Guid == releaseId);

            if (releaseToAddIteration == null)
            {
                return null;
            }

            if (releaseToAddIteration.Iterations.IsNullOrEmpty())
            {
                releaseToAddIteration.Iterations = new List<Iteration>();
            }

            releaseToAddIteration.Iterations.Add(newWorkspaceReleaseIteration);

            await _workspaceRepo.CreateOrUpdateWorkspace(workspaceFromRepo);

            return releaseToAddIteration;
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

        public async Task<Workspace> AddUserToWorkspace(Guid workspaceId, UserDto userDto)
        {
            var workspaceFromRepo = await _workspaceRepo.GetWorkspaceById(workspaceId);

            if (workspaceFromRepo == null)
            {
                return null;
            }

            var userFromRepo = await _userRepo.GetUserById(userDto.Guid);

            if (userFromRepo == null)
            {
                return null;
            }

            var newWorkspaceUser = _mapper.Map<User>(userDto);

            if (workspaceFromRepo.Users.IsNullOrEmpty())
            {
                workspaceFromRepo.Users = new List<User>();
            }

            workspaceFromRepo.Users.Add(newWorkspaceUser);

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

        public async Task<List<WorkspaceSummaryDto>> GetWorkspaceSummariesByUser(string username)
        {
            var workspacesByUser = await GetWorkspacesByUser(username);

            var workspaceSummaries = _mapper.Map<List<WorkspaceSummaryDto>>(workspacesByUser);

            return workspaceSummaries;
        }
    }
}