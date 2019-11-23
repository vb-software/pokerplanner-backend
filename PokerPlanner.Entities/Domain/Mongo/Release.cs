using System;
using System.Collections.Generic;

namespace PokerPlanner.Entities.Domain.Mongo
{
    [Serializable]
    public class Release
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Iteration> Iterations { get; set; }
    }
}