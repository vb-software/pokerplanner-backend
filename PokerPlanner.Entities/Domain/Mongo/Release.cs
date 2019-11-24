using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace PokerPlanner.Entities.Domain.Mongo
{
    public class Release
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [BsonIgnoreIfNull]
        public List<Iteration> Iterations { get; set; }
    }
}