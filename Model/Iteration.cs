namespace FLRC.Leaderboards.Model;

public record Iteration : Identifiable<Guid>
{
	public Guid ID { get; set; }
	public Guid SeriesID { get; set; }
	public string Name { get; set; } = null!;
	public DateOnly? StartDate { get; set; }
	public DateOnly? EndDate { get; set; }
	public string? RegistrationType { get; set; }
	public string? RegistrationContext { get; set; }

	public virtual Series Series { get; init; } = null!;
	public virtual ICollection<Race> Races { get; init; } = [];
	public virtual ICollection<Challenge> Challenges { get; init; } = [];
	public virtual ICollection<Athlete> Athletes { get; init; } = [];

	public bool IsActive
		=> DateOnly.FromDateTime(DateTime.Now) > StartDate && DateOnly.FromDateTime(DateTime.Now) <= EndDate;

	public Challenge? OfficialChallenge
		=> Challenges.FirstOrDefault(c => c is { IsOfficial: true, IsPrimary: true });
}