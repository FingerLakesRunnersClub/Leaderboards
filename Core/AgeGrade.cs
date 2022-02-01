using System;

namespace FLRC.Leaderboards.Core;

public record AgeGrade : Formatted<double>, IComparable<AgeGrade>
{
	public AgeGrade(double value) : base(value)
	{
	}

	public override string Display => Value > 0 ? $"{Value:F2}%" : string.Empty;

	public int CompareTo(AgeGrade other) => Value.CompareTo(other.Value);
}