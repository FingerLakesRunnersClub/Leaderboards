using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Overall;

public class OverallTeamResultsViewModel : OverallResultsViewModel
{
	public IReadOnlyCollection<TeamResults> Results { get; init; }
}