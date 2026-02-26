namespace FLRC.Leaderboards.Data.Models;

public sealed record LinkedAccount
{
	public Guid ID { get; init; }
	public Guid AthleteID { get; init; }
	public string Type { get; init; } = null!;
	public string Value { get; init; } = null!;

	public Athlete Athlete { get; init; } = null!;

	public static class Keys
	{
		public const string Discourse = nameof(Discourse);
		public const string Email = nameof(Email);
		public const string WebScorer = nameof(WebScorer);
	}
}