namespace FLRC.Leaderboards.Core;

public class TeamSummaryViewModel : ViewModel
{
	public override string Title => $"Team {Team.Display}";

	public Team Team { get; init; }
	public TeamResults Overall { get; init; }
	public IDictionary<Course, TeamResults> Courses { get; init; }
}