using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AutoWrapper.Extensions;
using AutoWrapper.Wrappers;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Repositories.Interfaces.Domain.Mongo;
using PokerPlanner.Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace PokerPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;

        public UsersController(IMapper mapper, IUserRepository userRepo)
        {
            _mapper = mapper;
            _userRepo = userRepo;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userRepo.GetUsers();
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<User> GetUserById(Guid id)
        {
            return await _userRepo.GetUserById(id);
        }

        [HttpPost]
        public async Task<ApiResponse> NewUser([FromBody] UserDto userDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepo.AddNewUser(_mapper.Map<User>(userDTO));

                return new ApiResponse("Created successfully", user, 201);
            }
            else
            {
                throw new ApiException(ModelState.AllErrors());
            }
        }
    }
}