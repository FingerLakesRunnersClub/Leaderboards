using System;

namespace FLRC.ChallengeDashboard
{
    public class Time : Formatted<TimeSpan>, IComparable<Time>
    {
        public Time(TimeSpan value) : base(value)
        {
        }

        public const string Format = @"h\:mm\:ss\.f";
        public override string Display => Value.ToString(Format);

        public Time Subtract(Time other) => new Time(Value - other.Value);

        public int CompareTo(Time other) => Value.CompareTo(other.Value);
    }
}