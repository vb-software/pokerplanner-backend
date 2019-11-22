using System;
using System.Text.Json.Serialization;
using PokerPlanner.Entities.Domain.Mongo.Attributes;

namespace PokerPlanner.Entities.Domain.Mongo
{
    [Serializable]
    [MongoDocument("users")]
    public class User : MongoIdentity
    {
        public Guid Guid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public byte[] PasswordHash { get; set; }
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}