namespace FLRC.Leaderboards.Core.Metrics;

public record SpeedComparison(double Value) : Formatted<double>(Value)
{
	private string Direction => Value < 0 ? "faster" : "slower";
	public override string Display => Value == 0 ? "Same" : $"{Math.Abs(Value):F1}% {Direction}";
}