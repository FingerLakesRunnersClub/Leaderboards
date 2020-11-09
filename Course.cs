using System;
using System.Collections.Generic;

namespace ChallengeDashboard
{
    public class Course
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public decimal Distance { get; set; }
        public string URL { get; set; }
        public ICollection<Result> Results { get; set; }
    }
}