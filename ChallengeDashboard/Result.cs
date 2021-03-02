using System;

namespace FLRC.ChallengeDashboard
{
    public class Result : IComparable<Result>
    {
        public uint CourseID { get; init; }
        public Athlete Athlete { get; init; }
        public Date StartTime { get; init; }
        public Time Duration { get; init; }
        
        public byte AgeOnDayOfRun => StartTime != null
            ? Athlete.AgeAsOf(StartTime.Value)
            : Athlete.Age;
        
        public int CompareTo(Result other) => Duration.CompareTo(other.Duration);

        public Time Behind(Result other) => Duration.Subtract(other.Duration);
    }
}