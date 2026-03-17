using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record ChallengeDashboard
{
	public Challenge Challenge { get; init; }
}