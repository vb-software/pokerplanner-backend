using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;
using PokerPlanner.Repositories.Interfaces.Domain.Mongo;
using PokerPlanner.Services.Interfaces.Domain.Mongo;

namespace PokerPlanner.API.Hubs
{
    public class GameHub : Hub
    {
        private readonly IUserRepository _userRepo;
        private readonly IWorkspaceService _workspaceService;
        private readonly IMapper _mapper;

        public GameHub(IUserRepository userRepo, IWorkspaceService workspaceService, IMapper mapper)
        {
            _userRepo = userRepo;
            _workspaceService = workspaceService;
            _mapper = mapper;
        }
        
        public async Task PlayerJoined(UserDto userDto, Game game)
        {
            // Check if guest
            if (userDto.Guid == Guid.Empty)
            {
                await Clients.All.SendAsync("GuestJoined", userDto);
            }

            // Get the user information
            var user = await _userRepo.GetUserById(userDto.Guid);

            await Clients.All.SendAsync("UserJoined", _mapper.Map<UserDto>(user));
        }

        public async Task PlayerUpdatedDisplayName(UserDto userDto)
        {
            await Clients.All.SendAsync("UserUpdatedDisplayName", userDto);
        }

        public async Task PlayerLeft(UserDto userDto)
        {
            await Clients.All.SendAsync("UserLeft", userDto);
        }

        public async Task UserPlacedScoreOnUserStory(UserDto userDto, int score)
        {
            // When a score is placed, all other users need to see that the user voted
            await Clients.All.SendAsync("ScorePostedOnStory", userDto, score);
        }

        public async Task CommenceVotingOnUserStory(UserStory userStory)
        {
            // Update the userstory on the iteration to be the only active story, blocking any other write access to the iteration
            await Clients.All.SendAsync("ActiveUserStory", userStory);
        }

        public async Task ConcludeVotingOnUserStory(UserStory userStory)
        {
            await Clients.All.SendAsync("VotingConcludedOnUserStory");
        }

        public async Task CreatorAddedUserStoryToGame(UserStory userStoryAdded, List<UserStory> userStories)
        {
            userStoryAdded.Guid = Guid.NewGuid();
            userStories.Add(userStoryAdded);

            // Broadcast the added user story to the game
            // However, there is no persistence should a user refresh their screen, need to figure this out
            await Clients.All.SendAsync("AddedUserStory", userStories);
        }
    }
}