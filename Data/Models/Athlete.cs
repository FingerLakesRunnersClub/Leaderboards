namespace FLRC.Leaderboards.Data.Models;

public sealed record Athlete
{
	public Guid ID { get; init; }
	public string Name { get; init; } = null!;
	public char Category { get; init; }
	public DateOnly DateOfBirth { get; init; }
	public bool IsPrivate { get; init; }

	public ICollection<Result> Results { get; init; } = [];
	public ICollection<LinkedAccount> LinkedAccounts { get; init; } = [];
}