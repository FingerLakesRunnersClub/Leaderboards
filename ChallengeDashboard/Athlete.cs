using System.Collections.Generic;

namespace ChallengeDashboard
{
    public class Athlete
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public byte Age { get; set; }
        public IEnumerable<Result> Results { get; set; }
    }
}