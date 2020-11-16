using System;

namespace ChallengeDashboard
{
    public class Result
    {
        public Course Course { get; set; }
        public Athlete Athlete { get; set; }
        public DateTime? StartTime { get; set; }
        public TimeSpan Duration { get; set; }
    }
}