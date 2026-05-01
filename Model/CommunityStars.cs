namespace FLRC.Leaderboards.Model;

public sealed record CommunityStars
{
	public Result Result { get; init; }
	public bool GroupRun { get; init; }
	public bool StoryPost { get; init; }
}