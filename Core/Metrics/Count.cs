namespace FLRC.Leaderboards.Core.Metrics;

public record Count(ushort Value) : Formatted<ushort>(Value), IComparable<Count>
{
	public override string Display
		=> Value.ToString("N0");

	public int CompareTo(Count other)
		=> Value.CompareTo(other.Value);
}