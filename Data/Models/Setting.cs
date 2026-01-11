namespace FLRC.Leaderboards.Data.Models;

public sealed record Setting : Setting<string>;

public abstract record Setting<T>
{
	public required Guid SeriesID { get; init; }
	public required string Name { get; init; }
	public required T Value { get; init; }

	public required Series Series { get; init; }
}