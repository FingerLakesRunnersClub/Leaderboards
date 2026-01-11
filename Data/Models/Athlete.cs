namespace FLRC.Leaderboards.Data.Models;

public sealed record Athlete
{
	public required Guid ID { get; init; }
	public required string Name { get; init; }
	public required char Category { get; init; }
	public required DateOnly DateOfBirth { get; init; }
	public required bool IsPrivate { get; init; }

	public Result[] Results { get; init; } = [];
	public LinkedAccount[] LinkedAccounts { get; init; } = [];
}