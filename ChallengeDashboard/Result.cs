using System;

namespace FLRC.ChallengeDashboard
{
    public class Result : IComparable<Result>, IComparable
    {
        public const string TimeFormat = @"h\:mm\:ss\.f";

        public Athlete Athlete { get; set; }
        public DateTime? StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string DisplayTime => Duration.ToString(TimeFormat);
        
        public int CompareTo(Result other) => Duration.CompareTo(other.Duration);
        public int CompareTo(object obj) => CompareTo(obj as Result);

        public TimeSpan Behind(Result other) => Duration - other.Duration;
    }
}