using System;

namespace FLRC.Leaderboards.Core;

public record Date : Formatted<DateTime>, IComparable<Date>
{
	public static readonly DateTime CompetitionStart = new(2021, 1, 1);

	public Date(DateTime value) : base(value)
	{
	}

	public override string Display => Value.ToLocalTime().ToString("M/d/yy h:mmtt").ToLower();

	public DateTime Week => new DateTime(Value.Year, 1, 1).AddDays(Math.Floor((Value.DayOfYear - 2) / 7.0) * 7 + 1);

	public int CompareTo(Date other) => Value.CompareTo(other.Value);
}