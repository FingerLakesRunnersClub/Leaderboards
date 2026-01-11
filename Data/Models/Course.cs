namespace FLRC.Leaderboards.Data.Models;

public sealed record Course
{
	public required Guid ID { get; init; }
	public required Guid RaceID { get; init; }
	public required decimal Distance { get; init; }
	public required string Units { get; init; }

	public required Race Race { get; init; }
	public Result[] Results { get; init; } = [];
	public Challenge[] Challenges { get; init; } = [];
}