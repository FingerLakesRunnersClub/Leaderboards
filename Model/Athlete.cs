namespace FLRC.Leaderboards.Model;

public sealed record Athlete : Identifiable<Guid>
{
	public const char UnknownCategory = ' ';
	private const double DaysPerYear = 365.2425;

	public Guid ID { get; set; }
	public string Name { get; set; } = null!;
	public char Category { get; set; }
	public DateOnly? DateOfBirth { get; set; }
	public bool IsPrivate { get; set; }

	public ICollection<Iteration> Registrations { get; init; } = [];
	public ICollection<Result> Results { get; init; } = [];
	public ICollection<LinkedAccount> LinkedAccounts { get; init; } = [];
	public ICollection<Challenge> Challenges { get; init; } = [];

	public byte? AgeAsOf(DateTime date) => DateOfBirth.HasValue
		? (byte)((date - new DateTime(DateOfBirth.Value.Year, DateOfBirth.Value.Month, DateOfBirth.Value.Day).ToUniversalTime()).TotalDays / DaysPerYear)
		: null;

	public byte? AgeToday => AgeAsOf(DateTime.Today);

	public ICollection<Admin> Admins { get; init; } = [];
	public bool IsAdmin => Admins.Count > 0;

	public bool HasLinkedAccount(string type)
		=> LinkedAccounts.Any(a => a.Type == type);

	public bool IsRegistered(Iteration iteration)
		=> Registrations.Contains(iteration);
}