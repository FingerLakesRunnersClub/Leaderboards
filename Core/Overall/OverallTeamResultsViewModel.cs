using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Overall;

public class OverallTeamResultsViewModel : OverallResultsViewModel
{
	public IEnumerable<TeamResults> Results { get; init; }
}