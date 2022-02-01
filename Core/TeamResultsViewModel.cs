namespace FLRC.Leaderboards.Core;

public class CourseTeamResultsViewModel : CourseResultsViewModel
{
	public IEnumerable<TeamResults> Results { get; init; }
}