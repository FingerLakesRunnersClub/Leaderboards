namespace FLRC.Leaderboards.Core.Metrics;

public record SpeedComparison : Formatted<double>
{
	public SpeedComparison(double value) : base(value)
	{
	}

	private string Direction => Value < 0 ? "faster" : "slower";
	public override string Display => Value == 0 ? "Same" : $"{Math.Abs(Value):F1}% {Direction}";
}