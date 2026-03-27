using System.Text.Json.Serialization;

namespace FLRC.Leaderboards.Model;

public record Setting : Setting<string>;

public abstract record Setting<T>
{
	public required Guid SeriesID { get; init; }
	public required string Key { get; init; }
	public required T Value { get; init; }

	[JsonIgnore]
	public virtual Series Series { get; init; } = null!;
}