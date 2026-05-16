namespace FLRC.Leaderboards.Core.Metrics;

public sealed record Count(int Value) : Formatted<int>(Value), IComparable<Count>
{
	public override string Display
		=> Value.ToString();

	public int CompareTo(Count other)
		=> Value.CompareTo(other.Value);
}