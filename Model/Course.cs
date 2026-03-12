namespace FLRC.Leaderboards.Model;

public sealed record Course : Identifiable<Guid>
{
	public Guid ID { get; set; }
	public Guid RaceID { get; set; }
	public decimal Distance { get; set; }
	public string Units { get; set; } = null!;
	public bool IsActive { get; set; }

	public Race Race { get; init; } = null!;
	public ICollection<Result> Results { get; init; } = [];
	public ICollection<Challenge> Challenges { get; init; } = [];

	public string DistanceDisplay => $"{Distance:#0.##} {Units}";
}