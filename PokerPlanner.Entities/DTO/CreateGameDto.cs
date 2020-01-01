using System;
using System.Collections.Generic;
using PokerPlanner.Entities.Domain.Mongo;

namespace PokerPlanner.Entities.DTO
{
    public class CreateGameDto
    {
        public bool IsPublic { get; set; }
        public Guid WorkspaceGuid { get; set; }
        public Guid ReleaseGuid { get; set; }
        public Guid IterationGuid { get; set; }
    }
}