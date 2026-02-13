namespace FLRC.Leaderboards.Data.Models;

public sealed record Athlete
{
	public static readonly DateOnly UnknownDOB = new();
	public const char UnknownCategory = ' ';

	public Guid ID { get; set; }
	public string Name { get; set; } = null!;
	public char Category { get; set; }
	public DateOnly DateOfBirth { get; set; }
	public bool IsPrivate { get; set; }

	public ICollection<Result> Results { get; init; } = [];
	public ICollection<LinkedAccount> LinkedAccounts { get; init; } = [];
}