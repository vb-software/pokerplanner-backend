using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using PokerPlanner.Entities.Domain.Mongo.Attributes;

namespace PokerPlanner.Entities.Domain.Mongo {
    [Serializable]
    [MongoDocument ("workspaces")]
    [BsonIgnoreExtraElements]
    public class Workspace : MongoIdentity {
        public Guid Guid { get; set; }
        public Guid OwnerGuid { get; set; }
        public string Name { get; set; }

        [BsonIgnoreIfNull]
        public Configuration Configuration { get; set; }

        [BsonIgnoreIfNull]
        public List<Release> Releases { get; set; }
    }
}