namespace FLRC.Leaderboards.Core.Metrics;

public record SpeedComparison(double Value) : Formatted<double>(Value)
{
	private string Direction => Value < 0 ? "faster" : "slower";
	public override string Display => Math.Abs(Value) < 0.1 ? "Same" : $"{Math.Abs(Value):F1}% {Direction}";
}