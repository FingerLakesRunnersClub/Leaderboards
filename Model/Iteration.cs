namespace FLRC.Leaderboards.Model;

public sealed record Iteration
{
	public Guid ID { get; set; }
	public Guid SeriesID { get; set; }
	public string Name { get; set; } = null!;
	public DateOnly? StartDate { get; set; }
	public DateOnly? EndDate { get; set; }
	public string? RegistrationType { get; set; }
	public string? RegistrationContext { get; set; }

	public Series Series { get; init; } = null!;
	public ICollection<Race> Races { get; init; } = [];
	public ICollection<Challenge> Challenges { get; init; } = [];
	public ICollection<Athlete> Athletes { get; init; } = [];
}