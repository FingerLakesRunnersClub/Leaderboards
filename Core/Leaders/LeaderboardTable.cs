using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Leaders;

public class LeaderboardTable
{
	public string Title { get; init; }
	public Course Course { get; init; }
	public FormattedResultType ResultType { get; init; }
	public Category Category { get; init; }
	public string Link { get; init; }
	public IEnumerable<LeaderboardRow> Rows { get; init; }
}