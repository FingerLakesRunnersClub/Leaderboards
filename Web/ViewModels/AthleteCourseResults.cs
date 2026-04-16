using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record AthleteCourseResults<T>
{
	public Athlete Athlete { get; init; }
	public Course Course { get; init; }

	public RankedList<T, Result> RankedResults { get; init; }

}