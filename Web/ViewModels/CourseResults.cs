using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record CourseResults<T>
{
	public Course Course { get; init; }
	public ResultType Type { get; init; }
	public RankedList<T,Model.Result> Results { get; init; }
}