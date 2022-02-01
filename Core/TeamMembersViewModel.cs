namespace FLRC.Leaderboards.Core;

public class TeamMembersViewModel : OverallResultsViewModel<TeamMember>
{
	public override string Title => $"Team {Team.Display} Members";

	public Team Team { get; init; }
}