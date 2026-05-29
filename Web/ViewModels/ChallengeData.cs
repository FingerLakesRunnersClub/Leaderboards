using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record ChallengeData
{
	public Challenge[] Challenges { get; init; }
	public Challenge Challenge { get; init; }
	public RankedList<ChallengeResult, Result> Results { get; init; }
}