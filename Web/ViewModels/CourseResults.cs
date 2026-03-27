using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed class CourseResults<T> : CourseResults
{
	public RankedList<T, Model.Result> Results { get; init; }

	public CourseResults ToBase() => this;
}

public class CourseResults
{
	public Course Course { get; init; }
	public ResultType Type { get; init; }
	public Filter Filter { get; init; }
	public bool HasAgeGroupTeams => true;
	public string ResponsiveBreakpoint => "md";
	public bool HasMultiAttemptCompetitions => true;
}