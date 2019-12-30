using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using PokerPlanner.Entities.AutoMapper.Profiles;
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
        private readonly WorkspaceService _service;

        public WorkspaceServiceTests()
        {
            _userRepo = new Mock<IUserRepository>();
            _workspaceRepo = new Mock<IWorkspaceRepository>();
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new WorkspaceMappings());
                cfg.AddProfile(new UserMappings());
            });
            var mapper = mockMapper.CreateMapper();
            _service = new WorkspaceService(_userRepo.Object, _workspaceRepo.Object, mapper);
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

            // _mapper.Setup(mapper => mapper.Map<Workspace>(createWorkspaceDto)).Returns(mappedWorkspace);

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

            _workspaceRepo.Setup(repo => repo.GetWorkspaceById(workspaceId)).ReturnsAsync(workspaceFromRepo);

            _workspaceRepo.Setup(repo => repo.CreateOrUpdateWorkspace(workspaceFromRepo)).ReturnsAsync(workspaceFromRepo);

            var workspace = await _service.AddReleaseToWorkspace(workspaceId, workspaceReleaseDto);

            Assert.NotNull(workspace);
            Assert.IsType<Workspace>(workspace);
            Assert.NotNull(workspace.Releases);
            Assert.Equal(1, workspace.Releases.Count);
        }

        [Fact]
        public async Task AddUserToWorkspaceWhenWorkspaceNullTest()
        {
            var workspaceId = Guid.NewGuid();

            var workspace = await _service.AddUserToWorkspace(workspaceId, new UserDto());

            Assert.Null(workspace);
        }

        [Fact]
        public async Task AddUserToWorkspaceWhenUserNullTest()
        {
            var workspaceId = Guid.NewGuid();
            var userDto = new UserDto();
            var workspaceFromRepo = new Workspace();

            _workspaceRepo.Setup(repo => repo.GetWorkspaceById(workspaceId)).ReturnsAsync(workspaceFromRepo);

            var workspace = await _service.AddUserToWorkspace(workspaceId, userDto);

            Assert.Null(workspace);
        }

        [Fact]
        public async Task AddUserToWorkspaceWhenUsersNullTest()
        {
            var workspaceId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var userDto = new UserDto { Guid = userId };
            var user = new User();
            var workspaceFromRepo = new Workspace();

            _userRepo.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);

            _workspaceRepo.Setup(repo => repo.GetWorkspaceById(workspaceId)).ReturnsAsync(workspaceFromRepo);

            _workspaceRepo.Setup(repo => repo.CreateOrUpdateWorkspace(workspaceFromRepo)).ReturnsAsync(workspaceFromRepo);

            var workspace = await _service.AddUserToWorkspace(workspaceId, userDto);

            Assert.NotNull(workspace);
            Assert.IsType<Workspace>(workspace);
            Assert.NotNull(workspace.Users);
            Assert.Equal(1, workspace.Users.Count);
        }
        
        [Fact]
        public async Task AddIterationToWorkspaceReleaseWhenWorkspaceNullTest()
        {
            var workspaceId = Guid.NewGuid();
            var releaseId = Guid.NewGuid();

            var release = await _service.AddIterationToWorkspaceRelease(workspaceId, releaseId, new CreateWorkspaceReleaseIterationDto());

            Assert.Null(release);
        }

        [Fact]
        public async Task AddIterationToWorkspaceReleaseWhenReleaseNullTest()
        {
            var workspaceId = Guid.NewGuid();
            var releaseId = Guid.NewGuid();
            var createIterationDto = new CreateWorkspaceReleaseIterationDto();
            var workspace = new Workspace { Releases = new List<Release> { new Release { Guid = Guid.NewGuid() } } };

            _workspaceRepo.Setup(repo => repo.GetWorkspaceById(workspaceId)).ReturnsAsync(workspace);

            var release = await _service.AddIterationToWorkspaceRelease(workspaceId, releaseId, createIterationDto);

            Assert.Null(release);
        }

        [Fact]
        public async Task AddIterationToWorkspaceReleaseWhenIterationsNullTest()
        {
            var workspaceId = Guid.NewGuid();
            var releaseId = Guid.NewGuid();
            var createIterationDto = new CreateWorkspaceReleaseIterationDto();
            var workspace = new Workspace { Releases = new List<Release> { new Release { Guid = releaseId } } };

            _workspaceRepo.Setup(repo => repo.GetWorkspaceById(workspaceId)).ReturnsAsync(workspace);

            var release = await _service.AddIterationToWorkspaceRelease(workspaceId, releaseId, createIterationDto);

            Assert.NotNull(release);
            Assert.IsType<Release>(release);
            Assert.NotNull(release.Iterations);
            Assert.Equal(1, release.Iterations.Count);
        }

        [Fact]
        public async Task GetWorkspaceWummariesByUserTest()
        {
            var username = "some-user";
            var userId = Guid.NewGuid();
            var user = new User { Guid = userId };
            List<Workspace> workspacesByUser = CreateWorkspacesByUser();

            _userRepo.Setup(repo => repo.UserExists(username)).ReturnsAsync(true);
            _userRepo.Setup(repo => repo.GetUserByUsername(username)).ReturnsAsync(user);

            _workspaceRepo.Setup(repo => repo.GetWorkspacesByUser(userId)).ReturnsAsync(workspacesByUser);

            var workspaceSummaries = await _service.GetWorkspaceSummariesByUser(username);

            Assert.NotNull(workspaceSummaries);
            Assert.IsType<List<WorkspaceSummaryDto>>(workspaceSummaries);
        }

        private static List<Workspace> CreateWorkspacesByUser()
        {
            return new List<Workspace>
            {
                new Workspace
                {
                    Configuration = new Configuration
                    {
                        AllowRevotes = true,
                        HideUserVotes = true,
                        ScoreSystem = ScoreSystem.Fibanocci
                    },
                    Users = new List<User>
                    {
                        new User
                        {
                            Username = "flinstone",
                            FirstName = "Fred",
                            LastName = "Flinstone",
                            Guid = Guid.NewGuid()
                        }
                    },
                    Guid = Guid.NewGuid(),
                    Name = "Some Workspace",
                    Releases = new List<Release>
                    {
                        new Release
                        {
                            Name = "My Release",
                            Iterations = new List<Iteration>
                            {
                                new Iteration
                                {
                                    Guid = Guid.NewGuid(),
                                    UserStories = new List<UserStory>
                                    {
                                        new UserStory
                                        {
                                            Description = "Some Description",
                                            Guid = Guid.NewGuid()
                                        }
                                    }
                                }
                            },
                            Guid = Guid.NewGuid()
                        }
                    }
                }
            };
        }
    }
}