using System;

namespace PokerPlanner.Entities.DTO
{
    public class WorkspaceSummaryDto
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }

        public bool HideUserVotes { get; set; }
        public bool AllowRevotes { get; set; }

        public string ScoreSystem { get; set; }

        public int AverageScore { get; set; }

        public int ReleasesCount { get; set; }

        public int UsersCount { get; set; }
    }
}