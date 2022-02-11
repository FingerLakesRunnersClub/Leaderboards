namespace FLRC.Leaderboards.Core.Metrics;

public record Points(double Value) : Formatted<double>(Value), IComparable<Points>
{
	public override string Display => Value.ToString("F2");

	public int CompareTo(Points other) => Value.CompareTo(other.Value);
}