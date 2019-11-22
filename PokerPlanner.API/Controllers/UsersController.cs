using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AutoWrapper.Extensions;
using AutoWrapper.Wrappers;
using PokerPlanner.Entities.Domain.Mongo;
using MongoDB.Bson;
using PokerPlanner.Repositories.Interfaces.Domain.Mongo;
using PokerPlanner.Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace PokerPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;
        private readonly IUserRepository _userRepo;

        public UsersController(IMapper mapper, ILogger<UsersController> logger, IUserRepository userRepo)
        {
            _mapper = mapper;
            _logger = logger;
            _userRepo = userRepo;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userRepo.GetUsers();
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<User> GetUserById(string id)
        {
            return await _userRepo.GetUserById(ObjectId.Parse(id));
        }

        [HttpPost]
        public async Task<ApiResponse> NewUser([FromBody] UserDto userDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userRepo.AddNewUser(_mapper.Map<User>(userDTO));
                    
                    return new ApiResponse("Created successfully", user, 201);
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex, "Error while trying to create user.");
                    throw;
                }
            }
            else
            {
                throw new ApiException(ModelState.AllErrors());
            }
        }
    }
}