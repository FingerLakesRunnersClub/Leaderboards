namespace FLRC.Leaderboards.Data.Models;

public sealed record Athlete
{
	public const char UnknownCategory = ' ';
	private const double DaysPerYear = 365.2425;

	public Guid ID { get; set; }
	public string Name { get; set; } = null!;
	public char Category { get; set; }
	public DateOnly? DateOfBirth { get; set; }
	public bool IsPrivate { get; set; }

	public ICollection<Result> Results { get; init; } = [];
	public ICollection<LinkedAccount> LinkedAccounts { get; init; } = [];

	public byte? AgeAsOf(DateTime date) => DateOfBirth.HasValue
		? (byte)((date - new DateTime(DateOfBirth.Value.Year, DateOfBirth.Value.Month, DateOfBirth.Value.Day).ToUniversalTime()).TotalDays / DaysPerYear)
		: null;

	public byte? AgeToday => AgeAsOf(DateTime.Today);
}