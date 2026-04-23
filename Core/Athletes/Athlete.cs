using System.Text.Json.Serialization;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Athletes;

public sealed record Athlete
{
	public uint ID { get; init; }
	public string Name { get; init; }

	public Category Category { get; init; }
	public bool Private { get; set; }

	public byte Age { get; init; }
	[JsonIgnore] public DateTime? DateOfBirth { get; init; }
	[JsonIgnore] public string Email { get; init; }

	public byte AgeAsOf(DateTime date) => DateOfBirth.HasValue
		? (byte) (date.Subtract(DateOfBirth.Value).TotalDays / Date.DaysPerYear)
		: Age;

	public Team Team
		=> Age switch
		{
			< 20 => Team.Teams[2],
			>= 70 => Team.Teams[6],
			_ => Team.Teams[(byte) (Age / 10)]
		};

	public byte AgeToday => AgeAsOf(DateTime.Today);

	public bool Equals(Athlete other) => ID == other?.ID;
	public override int GetHashCode() => (int) ID;
}