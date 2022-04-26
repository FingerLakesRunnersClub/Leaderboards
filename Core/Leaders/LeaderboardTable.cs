using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Leaders;

public class LeaderboardTable
{
	public string Link { get; init; }
	public string Title { get; init; }
	public Course Course { get; init; }
	public FormattedResultType ResultType { get; init; }
	public Filter Filter { get; init; }
	public Lazy<IReadOnlyCollection<LeaderboardRow>> Rows { get; init; }
}