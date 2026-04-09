using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed class ChallengeViewModel : DataTableViewModel
{
	public override string Title => Challenge.Name;

	public Challenge[] Challenges { get; init; }
	public Challenge Challenge { get; init; }
	public RankedList<ChallengeResult, Result> Results { get; init; }

	public override string ResponsiveBreakpoint => "xl";
}