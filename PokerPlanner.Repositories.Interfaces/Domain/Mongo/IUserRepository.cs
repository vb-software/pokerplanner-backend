using System.Collections.Generic;
using System.Threading.Tasks;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Repositories.Interfaces;
using MongoDB.Bson;
using System;

namespace PokerPlanner.Repositories.Interfaces.Domain.Mongo
{
    public interface IUserRepository : IRepository
    {
        Task<List<User>> GetUsers();
        Task<User> GetUserById(Guid userGuid);
        Task<User> GetUserByUsername(string username);
        Task RemoveUser(User userToRemove);
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}