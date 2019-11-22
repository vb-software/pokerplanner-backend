using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using Moq;
using PokerPlanner.API.Controllers;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Repositories.Interfaces.Domain.Mongo;
using Xunit;

namespace PokerPlanner.API.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _controller = new UsersController(null, null, _mockUserRepo.Object);
        }
        [Fact]
        public async Task GetUserByIdTest()
        {
            User mockUser = new User();
            var objectId = ObjectId.GenerateNewId().ToString();
            _mockUserRepo.Setup(repo => repo.GetUserById(ObjectId.Parse(objectId))).ReturnsAsync(mockUser);

            mockUser.Id = objectId;

            var result = await _controller.GetUserById(objectId);

            Assert.IsType<User>(result);
            Assert.Equal(objectId, result.Id);
        }
    }
}
