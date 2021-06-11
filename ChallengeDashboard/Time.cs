using System;

namespace FLRC.ChallengeDashboard
{
    public record Time : Formatted<TimeSpan>, IComparable<Time>
    {
        public Time(TimeSpan value) : base(value)
        {
        }

        public override string Display => Value.ToString(Value.TotalHours >= 1 ? @"h\:mm\:ss" : @"m\:ss");

        public Time Subtract(Time other) => new Time(Value - other.Value);

        public int CompareTo(Time other) => Value.CompareTo(other.Value);

        public override int GetHashCode() => Value.GetHashCode();
    }
}