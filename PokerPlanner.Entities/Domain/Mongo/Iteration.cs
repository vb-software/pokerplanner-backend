using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace PokerPlanner.Entities.Domain.Mongo
{
    public class Iteration
    {
        public Guid Guid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [BsonIgnoreIfNull]
        public List<UserStory> UserStories { get; set; }
    }
}