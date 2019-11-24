using System;

namespace PokerPlanner.Entities.Domain.Mongo
{
    [Serializable]
    public class UserStory
    {
        public Guid Guid { get; set; }
        public string Description { get; set; }
        public int Score { get; set; }
    }
}