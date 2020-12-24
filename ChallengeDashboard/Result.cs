using System;

namespace FLRC.ChallengeDashboard
{
    public class Result : IComparable<Result>
    {

        public Athlete Athlete { get; init; }
        public Date StartTime { get; init; }
        public Time Duration { get; init; }

        public int CompareTo(Result other) => Duration.CompareTo(other.Duration);

        public Time Behind(Result other) => Duration.Subtract(other.Duration);
    }
}