using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PokerPlanner.Entities.Domain.Mongo
{
    [Serializable]
    public abstract class MongoIdentity
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [JsonIgnore]
        public virtual ObjectId MongoId { get; set; }

        [BsonIgnore]
        // [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public virtual string Id
        {
            get { return MongoId.ToString() == ObjectId.Empty.ToString() ? null : MongoId.ToString(); }
            set { MongoId = ObjectId.Parse(value); }
        }
    }
}