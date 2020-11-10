using System;
using System.Collections.Generic;

namespace ChallengeDashboard
{
    public class Athlete
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public ICollection<Result> Results { get; set; }
    }
}