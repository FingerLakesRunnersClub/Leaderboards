using System.Text.Json.Serialization;

namespace FLRC.Leaderboards.Model;

public record Course : Identifiable<Guid>
{
	public Guid ID { get; set; }
	public Guid RaceID { get; set; }
	public decimal Distance { get; set; }
	public string Units { get; set; } = null!;
	public bool IsActive { get; set; }

	public virtual Race Race { get; init; } = null!;

	[JsonIgnore]
	public virtual ICollection<Result> Results { get; init; } = [];

	[JsonIgnore]
	public virtual ICollection<Challenge> Challenges { get; init; } = [];

	public string DistanceDisplay => $"{Distance:#0.##} {Units}";
}