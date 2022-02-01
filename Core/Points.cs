using System;

namespace FLRC.Leaderboards.Core;

public record Points : Formatted<double>, IComparable<Points>
{
	public Points(double value) : base(value)
	{
	}

	public override string Display => Value.ToString("F2");

	public int CompareTo(Points other) => Value.CompareTo(other.Value);
}