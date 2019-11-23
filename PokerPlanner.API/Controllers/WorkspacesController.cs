using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoWrapper.Extensions;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;
using PokerPlanner.Repositories.Interfaces.Domain.Mongo;
using PokerPlanner.Services.Interfaces.Domain.Mongo;

namespace PokerPlanner.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkspacesController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IWorkspaceService _workspaceService;

        public WorkspacesController(IUserRepository userRepo, IWorkspaceService workspaceService)
        {
            _userRepo = userRepo;
            _workspaceService = workspaceService;
        }

        [HttpPost]
        public async Task<ApiResponse> CreateWorkspace([FromBody] CreateWorkspaceDto workspaceDto)
        {
            var identity = User.Identity;

            if (ModelState.IsValid)
            {
                var workspace = await _workspaceService.CreateWorkspaceForUser(identity.Name, workspaceDto);

                return new ApiResponse("Workspace created successfully", workspace, (int)HttpStatusCode.OK);
            }
            else
            {
                throw new ApiException(ModelState.AllErrors());
            }
        }

        [HttpGet]
        public async Task<List<Workspace>> GetWorkspaces()
        {
            var identity = User.Identity;

            var workspaces = await _workspaceService.GetWorkspacesByUser(identity.Name);

            return workspaces;
        }

        [HttpGet("{workspaceId}")]
        public async Task<Workspace> GetWorkspaceById(Guid workspaceId)
        {
            var identity = User.Identity;

            var workspace = await _workspaceService.GetWorkspaceByUserAndId(identity.Name, workspaceId);

            return workspace;
        }
    }
}