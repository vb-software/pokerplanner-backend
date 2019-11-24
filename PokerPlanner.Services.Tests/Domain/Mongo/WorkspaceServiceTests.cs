using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;
using PokerPlanner.Repositories.Interfaces.Domain.Mongo;
using PokerPlanner.Services.Domain.Mongo;
using Xunit;

namespace PokerPlanner.Services.Tests.Domain.Mongo
{
    public class WorkspaceServiceTests
    {
        private readonly Mock<IUserRepository> _userRepo;
        private readonly Mock<IWorkspaceRepository> _workspaceRepo;
        private readonly Mock<IMapper> _mapper;
        private readonly WorkspaceService _service;

        public WorkspaceServiceTests()
        {
            _userRepo = new Mock<IUserRepository>();
            _workspaceRepo = new Mock<IWorkspaceRepository>();
            _mapper = new Mock<IMapper>();
            _service = new WorkspaceService(_userRepo.Object, _workspaceRepo.Object, _mapper.Object);
        }

        [Fact]
        public async Task CreateWorkspaceForUserNotExistTest()
        {
            var username = "some-user";

            _userRepo.Setup(repo => repo.UserExists(username)).ReturnsAsync(false);

            var workspaceCreated = await _service.CreateWorkspaceForUser(username, new CreateWorkspaceDto());

            Assert.Null(workspaceCreated);
        }

        [Fact]
        public async Task CreateWorkspaceForUserDoesExistTest()
        {
            var username = "some-user";

            _userRepo.Setup(repo => repo.UserExists(username)).ReturnsAsync(true);

            var createWorkspaceDto = new CreateWorkspaceDto { Name = "myworkspace" };
            var user = new User { Guid = Guid.NewGuid() };

            _userRepo.Setup(repo => repo.GetUserByUsername(username)).ReturnsAsync(user);

            var mappedWorkspace = new Workspace();

            _mapper.Setup(mapper => mapper.Map<Workspace>(createWorkspaceDto)).Returns(mappedWorkspace);

            var workspaceCreated = await _service.CreateWorkspaceForUser(username, createWorkspaceDto);

            Assert.NotNull(workspaceCreated);
            Assert.IsType<Workspace>(workspaceCreated);
            Assert.Equal(workspaceCreated.OwnerGuid, user.Guid);
            Assert.NotEqual(Guid.Empty, workspaceCreated.Guid);
        }

        [Fact]
        public async Task GetWorkspaceByUserAndIdUserNotExistTest()
        {
            var username = "some-user";
            var workspaceId = Guid.NewGuid();

            _userRepo.Setup(repo => repo.UserExists(username)).ReturnsAsync(false);

            var workspace = await _service.GetWorkspaceByUserAndId(username, workspaceId);

            Assert.Null(workspace);
        }

        [Fact]
        public async Task GetWorkspaceByUserAndIdUserDoesExistWithNonMatchingWorkspaceTest()
        {
            var username = "some-user";
            var userGuid = Guid.NewGuid();
            var workspaceId = Guid.NewGuid();

            var workspace = new Workspace { Guid = workspaceId };

            _userRepo.Setup(repo => repo.UserExists(username)).ReturnsAsync(true);
            _userRepo.Setup(repo => repo.GetUserByUsername(username)).ReturnsAsync(new User { Username = username, Guid = userGuid });

            var workspaceFound = await _service.GetWorkspaceByUserAndId(username, workspaceId);

            Assert.Null(workspaceFound);
        }

        [Fact]
        public async Task GetWorkspaceByUserAndIdUserDoesExistWithMatchingWorkspaceTest()
        {
            var username = "some-user";
            var userGuid = Guid.NewGuid();
            var workspaceId = Guid.NewGuid();

            var workspace = new Workspace { Guid = workspaceId };

            _userRepo.Setup(repo => repo.UserExists(username)).ReturnsAsync(true);
            _userRepo.Setup(repo => repo.GetUserByUsername(username)).ReturnsAsync(new User { Username = username, Guid = userGuid });

            _workspaceRepo.Setup(repo => repo.GetWorkspacesByUser(userGuid))
                .ReturnsAsync(new List<Workspace>
                {
                    new Workspace(),
                    workspace
                });

            var workspaceFound = await _service.GetWorkspaceByUserAndId(username, workspaceId);

            Assert.NotNull(workspaceFound);
            Assert.IsType<Workspace>(workspaceFound);
            Assert.Equal(workspaceId, workspaceFound.Guid);
        }

        [Fact]
        public async Task GetWorkspacesByUserNotExistTest()
        {
            var username = "some-user";

            _userRepo.Setup(repo => repo.UserExists(username)).ReturnsAsync(false);

            var workspaces = await _service.GetWorkspacesByUser(username);

            Assert.Null(workspaces);
        }

        [Fact]
        public async Task GetWorkspacesByUserDoesExistTest()
        {
            var username = "some-user";
            var userGuid = Guid.NewGuid();

            _userRepo.Setup(repo => repo.UserExists(username)).ReturnsAsync(true);
            _userRepo.Setup(repo => repo.GetUserByUsername(username)).ReturnsAsync(new User { Username = username, Guid = userGuid });

            _workspaceRepo.Setup(repo => repo.GetWorkspacesByUser(userGuid))
                .ReturnsAsync(new List<Workspace>
                {
                    new Workspace(),
                    new Workspace()
                });

            var workspaces = await _service.GetWorkspacesByUser(username);

            Assert.NotNull(workspaces);
            Assert.IsType<List<Workspace>>(workspaces);
            Assert.Equal(2, workspaces.Count);
        }

        [Fact]
        public async Task AddReleaseToWorkspaceWhenWorkspaceNullTest()
        {
            var workspaceId = Guid.NewGuid();

            var workspace = await _service.AddReleaseToWorkspace(workspaceId, new CreateWorkspaceReleaseDto());

            Assert.Null(workspace);
        }

        [Fact]
        public async Task AddReleaseToWorkspaceWhenReleasesNullTest()
        {
            var workspaceId = Guid.NewGuid();
            var workspaceReleaseDto = new CreateWorkspaceReleaseDto();
            var workspaceFromRepo = new Workspace();
            var mappedRelease = new Release();

            _workspaceRepo.Setup(repo => repo.GetWorkspaceById(workspaceId)).ReturnsAsync(workspaceFromRepo);

            _mapper.Setup(mapper => mapper.Map<Release>(workspaceReleaseDto)).Returns(mappedRelease);

            _workspaceRepo.Setup(repo => repo.CreateOrUpdateWorkspace(workspaceFromRepo)).ReturnsAsync(workspaceFromRepo);

            var workspace = await _service.AddReleaseToWorkspace(workspaceId, workspaceReleaseDto);

            Assert.NotNull(workspace);
            Assert.IsType<Workspace>(workspace);
            Assert.NotNull(workspace.Releases);
            Assert.Equal(mappedRelease, workspace.Releases.First());
        }
    }
}