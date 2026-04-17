using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Leaders;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record Leaderboard
{
	public IConfig Config { get; init; }
	public LeaderboardResultType LeaderboardResultType { get; init; }
	public LeaderboardTable[] OverallResults { get; init; }
	public IDictionary<Course, LeaderboardTable[]> OfficialCourses { get; init; }
	public IDictionary<Course, LeaderboardTable[]> OtherCourses { get; init; }
}