namespace FLRC.Leaderboards.Core.Metrics;

public record Miles(double Value) : Points(Value)
{
	public override string Display => Value.ToString("F1");
}