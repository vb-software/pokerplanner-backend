using System.Collections.Generic;
using System.Threading.Tasks;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.Settings;
using PokerPlanner.Repositories.Domain.Mongo.Extensions;
using PokerPlanner.Repositories.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using PokerPlanner.Repositories.Interfaces.Domain.Mongo;

namespace PokerPlanner.Repositories.Domain.Mongo
{
    public class UserRepository : MongoRepositoryBase, IUserRepository
    {
        public UserRepository(MongoDbSettings mongoDbSettings)
        {
            _mongoDbSettings = mongoDbSettings;
        }

        public async Task<User> AddNewUser(User userToAdd)
        {
            await GetCollection().InsertOneAsync(userToAdd);
            return userToAdd;
        }

        public async Task<User> GetUserById(ObjectId userId)
        {
            var filter = Builders<User>.Filter.Eq("_id", userId);
            var userFromRepo = await GetCollection().Find(user => user.MongoId == userId).FirstOrDefaultAsync();
            return userFromRepo;
        }

        public async Task<List<User>> GetUsers()
        {
            return await GetCollection().Find(x => true).ToListAsync();
        }

        public async Task RemoveUser(User userToRemove)
        {
            await GetCollection().DeleteOneAsync(user => user.MongoId == userToRemove.MongoId);
        }

        private IMongoCollection<User> GetCollection()
        {
            var collection = Db.GetGlobalCollection<User>();
            return collection;
        }

        public async Task<User> Login(string username, string password)
        {
            var userCollection = Db.GetGlobalCollection<User>();

            var user = await userCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeddHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computeddHash.Length; i++)
                {
                    if (computeddHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Guid = System.Guid.NewGuid();

            var userCollection = Db.GetGlobalCollection<User>();

            await userCollection.InsertOneAsync(user);

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            var userCollection = Db.GetGlobalCollection<User>();

            if (await userCollection.Find(x => x.Username == username).AnyAsync())
            {
                return true;
            }

            return false;
        }
    }
}