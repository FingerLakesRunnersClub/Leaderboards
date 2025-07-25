namespace FLRC.Leaderboards.Core.Metrics;

public record Time(TimeSpan Value) : Formatted<TimeSpan>(Value), IComparable<Time>
{
	public override string Display => Value.ToString(Value.TotalHours >= 1 ? @"h\:mm\:ss" : @"m\:ss");

	public Time Subtract(Time other) => new(Value - other.Value);

	public int CompareTo(Time other) => Value.CompareTo(other.Value);

	public override int GetHashCode() => Value.GetHashCode();

	public static double PercentDifference(Time mine, Time theirs)
		=> 100 * (theirs.Value.TotalSeconds - mine.Value.TotalSeconds) / mine.Value.TotalSeconds;

	public static double AbsolutePercentDifference(Time mine, Time theirs)
		=> Math.Abs(PercentDifference(mine, theirs));

	public static readonly Time Max = new(TimeSpan.MaxValue);
}