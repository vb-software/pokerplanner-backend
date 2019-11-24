using System;
using System.Collections.Generic;
using PokerPlanner.Entities.Domain.Mongo.Attributes;

namespace PokerPlanner.Entities.Domain.Mongo
{
    [Serializable]
    [MongoDocument("workspaces")]
    public class Workspace : MongoIdentity
    {
        public Guid Guid { get; set; }
        public Guid OwnerGuid { get; set; }
        public Configuration Configuration { get; set; }
        public List<Release> Releases { get; set; }
    }
}