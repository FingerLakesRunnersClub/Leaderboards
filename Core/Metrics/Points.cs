namespace FLRC.Leaderboards.Core.Metrics;

public record Points(double Value, byte Digits = 2) : Formatted<double>(Value), IComparable<Points>
{
	public override string Display => Value.ToString($"F{Digits}");

	public int CompareTo(Points other) => Value.CompareTo(other.Value);
}