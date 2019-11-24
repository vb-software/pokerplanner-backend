using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PokerPlanner.API.Controllers;
using PokerPlanner.Entities.Domain.Mongo;
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
            _controller = new WorkspacesController(_workspaceService.Object);
        }

        [Fact]
        public async Task GetWorkspaceById()
        {
            var workspaceId = Guid.NewGuid();
            _workspaceService.Setup(service => service.GetWorkspaceByUserAndId(It.IsAny<string>(), workspaceId)).ReturnsAsync(new Workspace());
        }

    }
}