using System.Text.Json.Serialization;

namespace FLRC.Leaderboards.Core.Races;

public sealed record Date : Formatted<DateTime>, IComparable<Date>
{
	public static readonly DateTime CompetitionStart = new(2021, 1, 1);

	[JsonIgnore]
	public DateOnly Week { get; }

	public Date(DateTime value) : base(value)
		=> Week = GetWeek();

	private DateOnly GetWeek()
	{
		var yearStart = new DateOnly(Value.Year, 1, 1);
		var firstSaturday = yearStart.AddDays(6 - (int) yearStart.DayOfWeek);
		return firstSaturday.AddDays((Value.DayOfYear - firstSaturday.DayOfYear) / 7 * 7);
	}

	public override string Display => Value.ToLocalTime().ToString("M/d/yy h:mmtt").ToLower();


	public int CompareTo(Date other) => Value.CompareTo(other.Value);
}