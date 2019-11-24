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
            _workspaceService.Setup(service => service.CreateWorkspaceForUser("username", createWorkspaceDto)).ReturnsAsync(workspace);

            var response = await _controller.CreateWorkspace(createWorkspaceDto);

            Assert.NotNull(response);
            Assert.IsType<ApiResponse>(response);
            Assert.Equal("Workspace created successfully", response.Message);
            Assert.False(response.IsError);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(workspace, response.Result);
        }

        [Fact]
        public async Task GetWorkspacesTest()
        {
            _workspaceService.Setup(service => service.GetWorkspacesByUser(It.IsAny<string>())).ReturnsAsync(new List<Workspace> { new Workspace() });

            var workspacesReturned = await _controller.GetWorkspaces();

            Assert.NotNull(workspacesReturned);
            Assert.IsType<List<Workspace>>(workspacesReturned);
        }

        [Fact]
        public async Task GetWorkspaceByIdTest()
        {
            var workspaceId = Guid.NewGuid();
            _workspaceService.Setup(service => service.GetWorkspaceByUserAndId(It.IsAny<string>(), workspaceId)).ReturnsAsync(new Workspace());

            var workspaceReturned = await _controller.GetWorkspaceById(workspaceId);

            Assert.NotNull(workspaceReturned);
            Assert.IsType<Workspace>(workspaceReturned);
        }

    }
}