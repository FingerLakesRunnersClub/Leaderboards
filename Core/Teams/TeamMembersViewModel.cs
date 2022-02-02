using FLRC.Leaderboards.Core.Overall;

namespace FLRC.Leaderboards.Core.Teams;

public class TeamMembersViewModel : OverallResultsViewModel<TeamMember>
{
	public override string Title => $"Team {Team.Display} Members";

	public Team Team { get; init; }
}