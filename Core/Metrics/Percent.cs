namespace FLRC.Leaderboards.Core.Metrics;

public sealed record Percent(double Value) : Formatted<double>(Value), IComparable<Percent>
{
	public override string Display => $"{Value:F0}%";
	public int CompareTo(Percent other) => Value.CompareTo(other.Value);
}