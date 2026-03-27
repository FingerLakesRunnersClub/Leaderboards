using System.Text.Json.Serialization;

namespace FLRC.Leaderboards.Model;

public record Challenge : Identifiable<Guid>
{
	public Guid ID { get; set; }
	public Guid IterationID { get; set; }
	public string Name { get; set; } = null!;
	public bool IsOfficial { get; set; }
	public bool IsPrimary { get; set; }
	public TimeSpan? TimeLimit { get; set; }
	public Guid? AthleteID { get; set; }

	public virtual Iteration Iteration { get; init; } = null!;

	[JsonIgnore]
	public virtual ICollection<Course> Courses { get; init; } = [];

	[JsonIgnore]
	public virtual ICollection<Athlete> Athletes { get; init; } = [];

	public virtual Athlete? Athlete { get; init; }

	public static class Types
	{
		public const string Default = nameof(Default);
		public const string Personal = nameof(Personal);
		public const string Ultra = nameof(Ultra);
	}
}