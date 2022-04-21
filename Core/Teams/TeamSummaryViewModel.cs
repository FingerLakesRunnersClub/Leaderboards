using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Teams;

public class TeamSummaryViewModel : ViewModel
{
	public override string Title => $"Team {Team.Display}";

	public Team Team { get; init; }
	public Ranked<TeamResults> Overall { get; init; }
	public IDictionary<Course, Ranked<TeamResults>> Courses { get; init; }
}