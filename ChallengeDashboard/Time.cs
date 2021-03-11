using System;

namespace FLRC.ChallengeDashboard
{
    public class Time : Formatted<TimeSpan>, IComparable<Time>, IEquatable<Time>
    {
        public Time(TimeSpan value) : base(value)
        {
        }

        public override string Display => Value.ToString(Value.TotalHours >= 1 ? @"h\:mm\:ss" : @"m\:ss");

        public Time Subtract(Time other) => new Time(Value - other.Value);

        public int CompareTo(Time other) => Value.CompareTo(other.Value);

        public bool Equals(Time other) => Value.Equals(other?.Value);
        
        public override bool Equals(object other) => Equals(other as Time);
    }
}