namespace FLRC.Leaderboards.Core.Metrics;

public record Stars(ushort Value) : Formatted<ushort>(Value), IComparable<Stars>
{
	public override string Display => Value.ToString("N0");

	public int CompareTo(Stars other) => Value.CompareTo(other.Value);
}