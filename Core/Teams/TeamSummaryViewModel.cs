using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Teams;

public sealed class TeamSummaryViewModel : ViewModel
{
	public override string Title => $"Team {Team.Display}";

	public Team Team { get; init; }
	public Ranked<TeamResults, Result> Overall { get; init; }
	public IDictionary<Course, Ranked<TeamResults, Result>> Courses { get; init; }
}