namespace FLRC.Leaderboards.Data.Models;

public sealed record Course
{
	public Guid ID { get; set; }
	public Guid RaceID { get; set; }
	public decimal Distance { get; set; }
	public string Units { get; set; } = null!;

	public Race Race { get; init; } = null!;
	public ICollection<Result> Results { get; init; } = [];
	public ICollection<Challenge> Challenges { get; init; } = [];

	public string DistanceDisplay => $"{Distance:#0.##} {Units}";
}