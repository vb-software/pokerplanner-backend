using System;
using System.Collections.Generic;

namespace PokerPlanner.Entities.Domain.Mongo
{
    [Serializable]
    public class Iteration
    {
        public Guid Guid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<UserStory> UserStories { get; set; }
    }
}