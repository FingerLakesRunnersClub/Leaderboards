using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Leaders;

public sealed class LeaderboardTable
{
	public string Link { get; init; }
	public string Title { get; init; }
	public Course Course { get; init; }
	public FormattedResultType ResultType { get; init; }
	public Filter Filter { get; init; }
	public Lazy<LeaderboardRow[]> Rows { get; init; }
}