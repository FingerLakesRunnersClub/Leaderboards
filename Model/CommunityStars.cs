namespace FLRC.Leaderboards.Model;

public sealed record CommunityStars
{
	public required Result Result { get; init; }
	public required bool GroupRun { get; init; }
	public required bool StoryPost { get; init; }

	public byte Score
		=> (byte)((GroupRun ? 1 : 0) + (StoryPost ? 1 : 0));
}