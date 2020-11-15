using System.Collections.Generic;

namespace ChallengeDashboard
{
    public class Course
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Distance { get; set; }
        public string URL { get; set; }
        public IEnumerable<Result> Results { get; set; }
    }
}