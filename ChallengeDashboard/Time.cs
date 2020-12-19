using System;

namespace FLRC.ChallengeDashboard
{
    public class Time : Formatted<TimeSpan>, IComparable<Time>
    {
        public Time(TimeSpan value) : base(value)
        {
        }

        public override string Display => Value.ToString(Value.TotalHours >= 1 ? @"h\:mm\:ss\.f" : @"m\:ss\.f");

        public Time Subtract(Time other) => new Time(Value - other.Value);

        public int CompareTo(Time other) => Value.CompareTo(other.Value);
    }
}