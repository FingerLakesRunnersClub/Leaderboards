using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public class TeamSummary
{
	public Team Team { get; init; }
	public Ranked<TeamResults, Result> Overall { get; init; }
	public IDictionary<Course, Ranked<TeamResults, Result>> Courses { get; init; }
}