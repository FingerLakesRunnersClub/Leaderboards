namespace FLRC.Leaderboards.Model;

public record RaceLink : Identifiable<Guid>
{
	public Guid ID { get; set; }
	public Guid RaceID { get; set; }
	public string Name { get; set; } = null!;
	public string Type { get; set; } = null!;
	public string URL { get; set; } = null!;

	public virtual Race Race { get; init; } = null!;

	public static class Types
	{
		public const string Info = nameof(Info);
		public const string Community = nameof(Community);
	}
}