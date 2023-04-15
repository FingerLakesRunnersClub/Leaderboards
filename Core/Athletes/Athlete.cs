using System.Collections.Immutable;
using System.Text.Json.Serialization;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Athletes;

public class Athlete : IEquatable<Athlete>
{
	public uint ID { get; init; }
	public string Name { get; init; }

	public Category Category { get; init; }

	public byte Age { get; init; }

	[JsonIgnore] public DateTime? DateOfBirth { get; init; }
	[JsonIgnore] public bool Private { get; set; }

	public byte AgeAsOf(DateTime date) => DateOfBirth.HasValue
		? (byte) (date.Subtract(DateOfBirth.Value).TotalDays / 365.2425)
		: Age;

	public Team Team => Age < 20 ? Teams[2]
		: Age >= 70 ? Teams[6]
		: Teams[(byte) (Age / 10)];

	public byte AgeToday => AgeAsOf(DateTime.Today);

	public static readonly IImmutableDictionary<byte, Team> Teams = Enumerable.Range(2, 5).ToImmutableDictionary(t => (byte) t, t => new Team((byte) t));

	public bool Equals(Athlete other) => ID == other?.ID;
	public override bool Equals(object obj) => Equals((Athlete) obj);
	public override int GetHashCode() => (int) ID;
}