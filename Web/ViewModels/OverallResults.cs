using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record OverallResults<T>
{
	public IDictionary<string,string> ResultTypes { get; init; }
	public string ResultType { get; init; }
	public RankedList<T, Result> Results { get; init; }
}