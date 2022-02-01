namespace FLRC.Leaderboards.Core;

public class OverallTeamResultsViewModel : OverallResultsViewModel
{
	public IEnumerable<TeamResults> Results { get; init; }
}