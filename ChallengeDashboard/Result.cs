using System;

namespace FLRC.ChallengeDashboard
{
    public class Result : IComparable<Result>, IComparable
    {

        public Athlete Athlete { get; set; }
        public DateTime? StartTime { get; set; }
        public Time Duration { get; set; }

        public int CompareTo(Result other) => Duration.CompareTo(other.Duration);
        public int CompareTo(object obj) => CompareTo(obj as Result);

        public Time Behind(Result other) => Duration.Subtract(other.Duration);
    }
}