using System.Collections.Generic;

namespace FLRC.ChallengeDashboard;

public class CourseTeamResultsViewModel : CourseResultsViewModel
{
	public IEnumerable<TeamResults> Results { get; init; }
}
