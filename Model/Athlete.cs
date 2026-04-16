using System.Text.Json.Serialization;

namespace FLRC.Leaderboards.Model;

public record Athlete : Identifiable<Guid>
{
	public const char UnknownCategory = ' ';
	private const double DaysPerYear = 365.2425;

	public Guid ID { get; set; }
	public string Name { get; set; } = null!;
	public char Category { get; set; }
	public DateOnly? DateOfBirth { get; set; }
	public bool IsPrivate { get; set; }

	[JsonIgnore]
	public virtual ICollection<Iteration> Registrations { get; init; } = [];

	[JsonIgnore]
	public virtual ICollection<Result> Results { get; init; } = [];

	[JsonIgnore]
	public virtual ICollection<LinkedAccount> LinkedAccounts { get; init; } = [];

	[JsonIgnore]
	public virtual ICollection<Challenge> Challenges { get; init; } = [];

	public byte? AgeAsOf(DateOnly date)
		=> AgeAsOf(date.ToDateTime(TimeOnly.MinValue));

	public byte? AgeAsOf(DateTime date)
		=> DateOfBirth.HasValue
			? (byte)((date - new DateTime(DateOfBirth.Value.Year, DateOfBirth.Value.Month, DateOfBirth.Value.Day).ToUniversalTime()).TotalDays / DaysPerYear)
			: null;

	public byte? AgeToday => AgeAsOf(DateTime.Today);

	public bool HasLinkedAccount(string type)
		=> LinkedAccounts.Any(a => a.Type == type);

	public bool IsRegistered(Iteration iteration)
		=> Registrations.Contains(iteration);

	public bool HasChallenge(Iteration iteration)
		=> Challenges.Any(c => c.Iteration == iteration);
}