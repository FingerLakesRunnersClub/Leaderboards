using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record TeamMembers : OverallResults<TeamMember>
{
	public Team Team { get; init; }
}