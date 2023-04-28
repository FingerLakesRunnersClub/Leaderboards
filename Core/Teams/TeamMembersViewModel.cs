using FLRC.Leaderboards.Core.Overall;

namespace FLRC.Leaderboards.Core.Teams;

public sealed class TeamMembersViewModel : OverallResultsViewModel<TeamMember>
{
	public override string Title => $"Team {Team.Display} Members";

	public Team Team { get; init; }
}