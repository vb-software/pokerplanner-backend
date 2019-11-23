using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;
using PokerPlanner.Entities.Settings;
using PokerPlanner.Repositories.Interfaces.Domain.Mongo;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using AutoWrapper.Extensions;
using Microsoft.Extensions.Logging;
using System.Net;

namespace PokerPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserRepository _userRepo;
        private readonly JwtSettings _settings;

        public AuthController(
            IUserRepository userRepo, 
            JwtSettings settings,
            ILogger<AuthController> logger)
        {
            _userRepo = userRepo;
            _settings = settings;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ApiResponse> Register([FromBody] UserForRegisterDto userForRegisterDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(userForRegisterDto.Username))
                    {
                        userForRegisterDto.Username = userForRegisterDto.Username.Trim().ToLower();
                    }

                    if (await _userRepo.UserExists(userForRegisterDto.Username))
                    {
                        return new ApiResponse((int)HttpStatusCode.BadRequest, "User already exists with that username");
                    }

                    var userToCreate = new User
                    {
                        Username = userForRegisterDto.Username,
                        FirstName = userForRegisterDto.FirstName.Trim(),
                        LastName = userForRegisterDto.LastName.Trim()
                    };

                    var trimmedPassword = userForRegisterDto.Password.Trim();

                    var createUser = await _userRepo.Register(userToCreate, trimmedPassword);

                    return new ApiResponse("Created successfully", createUser, 201);
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _userRepo.Login(userForLoginDto.Username.Trim().ToLower(), userForLoginDto.Password.Trim());

            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            // generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userFromRepo.Guid.ToString()),
                    new Claim(ClaimTypes.Name, userFromRepo.Username),
                    new Claim(ClaimTypes.GivenName, $"{userFromRepo.FirstName} {userFromRepo.LastName}" )
                }),
                Expires = DateTime.Now.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { tokenString });
        }
    }
}