using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Leaders;

public sealed class LeaderboardRow
{
	public Rank Rank { get; init; }
	public string Link { get; init; }
	public string Name { get; init; }
	public string Value { get; init; }
}