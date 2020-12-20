using System;

namespace FLRC.ChallengeDashboard
{
    public class Result : IComparable<Result>
    {

        public Athlete Athlete { get; set; }
        public DateTime? StartTime { get; set; }
        public Time Duration { get; set; }

        public int CompareTo(Result other) => Duration.CompareTo(other.Duration);

        public Time Behind(Result other) => Duration.Subtract(other.Duration);
    }
}