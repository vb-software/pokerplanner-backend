using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PokerPlanner.API.Controllers;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;
using PokerPlanner.Services.Interfaces.Domain.Mongo;
using Xunit;

namespace PokerPlanner.API.Tests.Controllers
{
    public class WorkspacesControllerTests
    {
        private readonly Mock<IWorkspaceService> _workspaceService;
        private readonly WorkspacesController _controller;
        private string _username;

        public WorkspacesControllerTests()
        {
            _workspaceService = new Mock<IWorkspaceService>();
            _controller = SetupControllerWithClaimsPrincipal();
        }

        private WorkspacesController SetupControllerWithClaimsPrincipal()
        {
            var controller = new WorkspacesController(_workspaceService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, "username")
                }, "someAuthTypeName"))
                }
            };

            _username = controller.User.Identity.Name;

            return controller;
        }

        [Fact]
        public async Task CreateWorkSpaceInvalidModelTest()
        {
            _controller.ModelState.AddModelError("error", "error");

            try
            {
                var response = await _controller.CreateWorkspace(new CreateWorkspaceDto());
            }
            catch (Exception e)
            {
                Assert.IsType<ApiException>(e);
            }
        }

        [Fact]
        public async Task CreateWorkSpaceValidModelTest()
        {
            var createWorkspaceDto = new CreateWorkspaceDto();
            var workspace = new Workspace();
            _workspaceService.Setup(service => service.CreateWorkspaceForUser(_username, createWorkspaceDto)).ReturnsAsync(workspace);

            var response = await _controller.CreateWorkspace(createWorkspaceDto);

            Assert.NotNull(response);
            Assert.IsType<ApiResponse>(response);
            Assert.Equal("Workspace created successfully", response.Message);
            Assert.False(response.IsError);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(workspace, response.Result);
        }

        [Fact]
        public async Task UpdateWorkSpaceInvalidModelTest()
        {
            _controller.ModelState.AddModelError("error", "error");

            try
            {
                var response = await _controller.UpdateWorkspace(new CreateWorkspaceDto());
            }
            catch (Exception e)
            {
                Assert.IsType<ApiException>(e);
            }
        }

        [Fact]
        public async Task UpdateWorkSpaceValidModelTest()
        {
            var createWorkspaceDto = new CreateWorkspaceDto();
            var workspace = new Workspace();
            _workspaceService.Setup(service => service.CreateWorkspaceForUser(_username, createWorkspaceDto)).ReturnsAsync(workspace);

            var response = await _controller.UpdateWorkspace(createWorkspaceDto);

            Assert.NotNull(response);
            Assert.IsType<ApiResponse>(response);
            Assert.Equal("Workspace updated successfully", response.Message);
            Assert.False(response.IsError);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(workspace, response.Result);
        }

        [Fact]
        public async Task GetWorkspacesTest()
        {
            _workspaceService.Setup(service => service.GetWorkspacesByUser(_username)).ReturnsAsync(new List<Workspace> { new Workspace() });

            var workspacesReturned = await _controller.GetWorkspaces();

            Assert.NotNull(workspacesReturned);
            Assert.IsType<List<Workspace>>(workspacesReturned);
        }

        [Fact]
        public async Task GetWorkspaceByIdTest()
        {
            var workspaceId = Guid.NewGuid();
            _workspaceService.Setup(service => service.GetWorkspaceByUserAndId(_username, workspaceId)).ReturnsAsync(new Workspace());

            var workspaceReturned = await _controller.GetWorkspaceById(workspaceId);

            Assert.NotNull(workspaceReturned);
            Assert.IsType<Workspace>(workspaceReturned);
        }

        [Fact]
        public async Task AddReleaseToWorkspaceInvalidModelTest()
        {
            _controller.ModelState.AddModelError("error", "error");

            try
            {
                await _controller.AddReleaseToWorkspace(Guid.NewGuid(), new CreateWorkspaceReleaseDto());
            }
            catch (Exception e)
            {
                Assert.IsType<ApiException>(e);
            }
        }

        [Fact]
        public async Task AddReleaseToWorkspaceValidModelTest()
        {
            var workspaceId = Guid.NewGuid();
            var workspaceReleaseDto = new CreateWorkspaceReleaseDto();
            var workspace = new Workspace();

            _workspaceService.Setup(service => service.AddReleaseToWorkspace(workspaceId, workspaceReleaseDto)).ReturnsAsync(workspace);

            var response = await _controller.AddReleaseToWorkspace(workspaceId, workspaceReleaseDto);

            Assert.NotNull(response);
            Assert.IsType<ApiResponse>(response);
            Assert.Equal("Release added to workspace successfully", response.Message);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(workspace, response.Result);
        }

        [Fact]
        public async Task AddIterationToWorkspaceReleaseInvalidModelTest()
        {
            _controller.ModelState.AddModelError("error", "error");

            try
            {
                await _controller.AddIterationToWorkspaceRelease(Guid.NewGuid(), Guid.NewGuid(), new CreateWorkspaceReleaseIterationDto());
            }
            catch (Exception e)
            {
                Assert.IsType<ApiException>(e);
            }
        }

        [Fact]
        public async Task AddIterationToWorkspaceReleaseValidModelTest()
        {
            var workspaceId = Guid.NewGuid();
            var workspaceReleaseIterationDto = new CreateWorkspaceReleaseIterationDto();
            var releaseId = Guid.NewGuid();
            var release = new Release();

            _workspaceService.Setup(service => service.AddIterationToWorkspaceRelease(workspaceId, releaseId, workspaceReleaseIterationDto)).ReturnsAsync(release);

            var response = await _controller.AddIterationToWorkspaceRelease(workspaceId, releaseId, workspaceReleaseIterationDto);

            Assert.NotNull(response);
            Assert.IsType<ApiResponse>(response);
            Assert.Equal("Iteration added to release successfully", response.Message);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(release, response.Result);
        }

        [Fact]
        public async Task GetWorkspaceSummariesTest()
        {
            var workspaceSummaries = new List<WorkspaceSummaryDto>();

            _workspaceService.Setup(service => service.GetWorkspaceSummariesByUser(_username)).ReturnsAsync(workspaceSummaries);

            var response = await _controller.GetWorkspaceSummaries();

            Assert.NotNull(response);
            Assert.IsType<List<WorkspaceSummaryDto>>(response);
        }

        [Fact]
        public async Task AddUserToWorkspaceInvalidModelTest()
        {
            _controller.ModelState.AddModelError("error", "error");

            try
            {
                await _controller.AddUserToWorkspace(Guid.NewGuid(), new UserDto());
            }
            catch (Exception e)
            {
                Assert.IsType<ApiException>(e);
            }
        }

        [Fact]
        public async Task AddUserToWorkspaceValidModelTest()
        {
            var userDto = new UserDto();
            var workspaceId = Guid.NewGuid();
            var workspace = new Workspace();

            _workspaceService.Setup(service => service.AddUserToWorkspace(workspaceId, userDto)).ReturnsAsync(workspace);

            var response = await _controller.AddUserToWorkspace(workspaceId, userDto);

            Assert.NotNull(response);
            Assert.IsType<Workspace>(response.Result);
        }

    }
}