namespace FLRC.Leaderboards.Model;

public sealed record LinkedAccount : Identifiable<Guid>
{
	public Guid ID { get; set; }
	public Guid AthleteID { get; set; }
	public string Type { get; set; } = null!;
	public string Value { get; set; } = null!;

	public Athlete Athlete { get; init; } = null!;

	public static class Keys
	{
		public const string Discourse = nameof(Discourse);
		public const string Email = nameof(Email);
		public const string WebScorer = nameof(WebScorer);
	}

	public static readonly LinkedAccountComparer Comparer = new();
}