using System.Text.Json.Serialization;

namespace FLRC.Leaderboards.Model;

public record Athlete : Identifiable<Guid>
{
	public const char UnknownCategory = ' ';

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
	{
		if (!DateOfBirth.HasValue)
			return null;

		var years = date.Year - DateOfBirth.Value.Year;
		return date < DateOfBirth.Value.ToDateTime(TimeOnly.MinValue).AddYears(years)
			? (byte)(years - 1)
			: (byte)years;
	}

	public byte? AgeToday => AgeAsOf(DateTime.Today);

	public bool HasLinkedAccount(string type)
		=> LinkedAccounts.Any(a => a.Type == type);

	public bool IsRegistered(Iteration iteration)
		=> Registrations.Contains(iteration);

	public Challenge? Challenge(Iteration iteration)
		=> Challenges.FirstOrDefault(c => c.Iteration == iteration);

	public bool HasChallenge(Iteration iteration)
		=> Challenge(iteration) is not null;
}