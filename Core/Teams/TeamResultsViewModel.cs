using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Teams;

public class CourseTeamResultsViewModel : CourseResultsViewModel
{
	public IEnumerable<TeamResults> Results { get; init; }
}