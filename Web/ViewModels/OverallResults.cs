using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record OverallResults<T> : OverallResults
{
	public RankedList<T, Result> RankedResults { get; init; }
}

public record OverallResults
{
	public IConfig Config { get; init; }
	public string ResultType { get; init; }
}