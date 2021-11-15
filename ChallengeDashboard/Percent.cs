using System;

namespace FLRC.ChallengeDashboard;

public record Percent : Formatted<double>, IComparable<Percent>
{
	public Percent(double value) : base(value)
	{
	}

	public override string Display => $"{Value:F0}%";
	public int CompareTo(Percent other) => Value.CompareTo(other.Value);
}
