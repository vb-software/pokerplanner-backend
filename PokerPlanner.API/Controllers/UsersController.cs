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
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
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
    }
}