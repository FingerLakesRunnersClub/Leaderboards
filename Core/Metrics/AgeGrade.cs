namespace FLRC.Leaderboards.Core.Metrics;

public record AgeGrade(double Value) : Formatted<double>(Value), IComparable<AgeGrade>
{
	public override string Display => Value > 0 ? $"{Value:F2}%" : string.Empty;

	public int CompareTo(AgeGrade other) => Value.CompareTo(other.Value);
}