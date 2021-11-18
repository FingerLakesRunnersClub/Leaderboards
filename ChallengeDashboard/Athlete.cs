using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace FLRC.ChallengeDashboard;

public class Athlete
{
	public uint ID { get; init; }
	public string Name { get; init; }

	public Category Category { get; init; }

	public byte Age { get; init; }

	[JsonIgnore]
	public DateTime DateOfBirth { get; init; }

	public byte AgeAsOf(DateTime date) => (byte)(date.Subtract(DateOfBirth).TotalDays / 365.2425);

	public Team Team => Age < 20 ? Teams[2]
		: Age >= 70 ? Teams[6]
		: Teams[(byte)(Age / 10)];

	public byte AgeToday => AgeAsOf(DateTime.Today);

	public static readonly IImmutableDictionary<byte, Team> Teams = Enumerable.Range(2, 5).ToImmutableDictionary(t => (byte)t, t => new Team((byte)t));
}