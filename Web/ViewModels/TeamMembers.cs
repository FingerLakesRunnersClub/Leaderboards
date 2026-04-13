using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record TeamMembers
{
	public Team Team { get; init; }
	public RankedList<TeamMember, Result> Results { get; init; }
}