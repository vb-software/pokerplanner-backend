using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoWrapper.Extensions;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;
using PokerPlanner.Services.Interfaces.Domain.Mongo;

namespace PokerPlanner.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkspacesController : ControllerBase
    {
        private readonly IWorkspaceService _workspaceService;

        public WorkspacesController(IWorkspaceService workspaceService)
        {
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

        [HttpPut]
        public async Task<ApiResponse> UpdateWorkspace([FromBody] CreateWorkspaceDto workspaceDto)
        {
            var identity = User.Identity;

            if (ModelState.IsValid)
            {
                var workspace = await _workspaceService.CreateWorkspaceForUser(identity.Name, workspaceDto);

                return new ApiResponse("Workspace updated successfully", workspace, (int)HttpStatusCode.OK);
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

        [HttpGet("summaries")]
        public async Task<List<WorkspaceSummaryDto>> GetWorkspaceSummaries()
        {
            var identity = User.Identity;

            var workspaceSummaries = await _workspaceService.GetWorkspaceSummariesByUser(identity.Name);

            return workspaceSummaries;
        }

        [HttpGet("{workspaceId}")]
        public async Task<Workspace> GetWorkspaceById(Guid workspaceId)
        {
            var identity = User.Identity;

            var workspace = await _workspaceService.GetWorkspaceByUserAndId(identity.Name, workspaceId);

            return workspace;
        }

        [HttpPost("{workspaceId}")]
        public async Task<ApiResponse> AddUserToWorkspace(Guid workspaceId, [FromBody] UserDto user)
        {
            if (ModelState.IsValid)
            {
                var workspace = await _workspaceService.AddUserToWorkspace(workspaceId, user);

                return new ApiResponse("User added to workspace successfully", workspace, (int)HttpStatusCode.OK);
            }
            else
            {
                throw new ApiException(ModelState.AllErrors());
            }
        }

        [HttpPost("{workspaceId}/releases")]
        public async Task<ApiResponse> AddReleaseToWorkspace(Guid workspaceId, [FromBody] CreateWorkspaceReleaseDto workspaceReleaseDto)
        {
            if (ModelState.IsValid)
            {
                var workspace = await _workspaceService.AddReleaseToWorkspace(workspaceId, workspaceReleaseDto);

                return new ApiResponse("Release added to workspace successfully", workspace, (int)HttpStatusCode.OK);
            }
            else
            {
                throw new ApiException(ModelState.AllErrors());
            }
        }

        [HttpPost("{workspaceId}/releases/{releaseId}/iterations")]
        public async Task<ApiResponse> AddIterationToWorkspaceRelease(Guid workspaceId, Guid releaseId, [FromBody] CreateWorkspaceReleaseIterationDto workspaceReleaseIterationDto)
        {
            if (ModelState.IsValid)
            {
                var release = await _workspaceService.AddIterationToWorkspaceRelease(workspaceId, releaseId, workspaceReleaseIterationDto);

                return new ApiResponse("Iteration added to release successfully", release, (int)HttpStatusCode.OK);
            }
            else
            {
                throw new ApiException(ModelState.AllErrors());
            }
        }
    }
}