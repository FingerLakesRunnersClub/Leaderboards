using FLRC.Leaderboards.Core.Leaders;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed class LeaderboardTable
{
	public string Link { get; init; }
	public string Title { get; init; }
	public Course Course { get; init; }
	public FormattedResultType ResultType { get; init; }
	public Filter Filter { get; init; }
	public Lazy<LeaderboardRow[]> Rows { get; init; }
}