using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using PokerPlanner.Entities.Domain.Mongo.Attributes;

namespace PokerPlanner.Entities.Domain.Mongo
{
    [Serializable]
    [MongoDocument("games")]
    public class Game : MongoIdentity
    {
        public Guid Guid { get; set; }
        [BsonIgnoreIfDefault]
        public Guid CreatorGuid { get; set; }
        [BsonIgnoreIfDefault]
        public bool IsPublic { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        [BsonIgnoreIfDefault]
        public Guid WorkspaceGuid { get; set; }
        [BsonIgnoreIfDefault]
        public Guid ReleaseGuid { get; set; }

        [BsonIgnoreIfNull]
        public Iteration Iteration { get; set; }
    }
}