namespace FLRC.Leaderboards.Data.Models;

public sealed record Series
{
	public required Guid ID { get; init; }
	public required string Key { get; init; }
	public required string Name { get; init; }

	public Setting[] Settings { get; init; } = [];
	public Feature[] Features { get; init; } = [];
	public Iteration[] Iterations { get; init; } = [];
}