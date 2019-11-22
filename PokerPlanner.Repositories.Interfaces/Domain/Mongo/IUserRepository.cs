using System.Collections.Generic;
using System.Threading.Tasks;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Repositories.Interfaces;
using MongoDB.Bson;

namespace PokerPlanner.Repositories.Interfaces.Domain.Mongo
{
    public interface IUserRepository : IRepository
    {
        Task<List<User>> GetUsers();
        Task<User> GetUserById(ObjectId userId);
        Task<User> AddNewUser(User userToAdd);
        Task RemoveUser(User userToRemove);
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}