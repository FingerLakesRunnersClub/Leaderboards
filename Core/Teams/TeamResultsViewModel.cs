using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Teams;

public class CourseTeamResultsViewModel : CourseResultsViewModel
{
	public IReadOnlyCollection<TeamResults> Results { get; init; }
}