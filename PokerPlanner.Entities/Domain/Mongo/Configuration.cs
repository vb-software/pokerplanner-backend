using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PokerPlanner.Entities.Domain.Mongo
{
    public enum ScoreSystem : byte
    {
        Fibanocci
    }

    public class Configuration
    {
        public Guid Guid { get; set; }
        public bool HideUserVotes { get; set; }
        public bool AllowRevotes { get; set; }
        [BsonRepresentation(BsonType.String)]
        public ScoreSystem ScoreSystem { get; set; }
    }
}