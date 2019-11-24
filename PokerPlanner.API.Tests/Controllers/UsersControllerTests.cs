using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoWrapper.Wrappers;
using MongoDB.Bson;
using Moq;
using PokerPlanner.API.Controllers;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;
using PokerPlanner.Repositories.Interfaces.Domain.Mongo;
using Xunit;

namespace PokerPlanner.API.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new UsersController(_mockMapper.Object, _mockUserRepo.Object);
        }
        [Fact]
        public async Task GetUserByIdTest()
        {
            User mockUser = new User();
            var objectId = ObjectId.GenerateNewId().ToString();
            var mockGuid = Guid.NewGuid();
            _mockUserRepo.Setup(repo => repo.GetUserById(mockGuid)).ReturnsAsync(mockUser);

            mockUser.Id = objectId;

            var result = await _controller.GetUserById(mockGuid);

            Assert.IsType<User>(result);
            Assert.Equal(objectId, result.Id);
        }

        [Fact]
        public async Task GetAllUsersTest()
        {
            _mockUserRepo.Setup(repo => repo.GetUsers()).ReturnsAsync(new List<User> { new User(), new User(), new User() });

            var result = await _controller.GetAllUsers();

            Assert.IsType<List<User>>(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task NewUserInvalidModelTest()
        {
            var newUserDto = new UserDto();

            _controller.ModelState.AddModelError("error", "error");

            try
            {
                var result = await _controller.NewUser(newUserDto);
            }
            catch (Exception e)
            {
                Assert.IsType<ApiException>(e);
            }
        }

        [Fact]
        public async Task NewUserSuccessfullyCreatedTest()
        {
            var newUserDto = new UserDto();
            var mappedUser = new User();

            _mockMapper.Setup(map => map.Map<User>(newUserDto)).Returns(mappedUser);
            _mockUserRepo.Setup(repo => repo.AddNewUser(mappedUser)).ReturnsAsync(mappedUser);

            var result = await _controller.NewUser(newUserDto);

            Assert.IsType<ApiResponse>(result);
            Assert.Equal(201, result.StatusCode);
            Assert.IsType<User>(result.Result);
            Assert.Equal(mappedUser, result.Result);
        }
    }
}