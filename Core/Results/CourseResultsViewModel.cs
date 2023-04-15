using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Results;

public class CourseResultsViewModel<T> : CourseResultsViewModel
{
	public RankedList<T> RankedResults { get; init; }
}

public class CourseResultsViewModel : DataTableViewModel
{
	public override string Title => $"{ResultType.Display} — {Course.Name}";

	public FormattedResultType ResultType { get; init; }
	public Filter Filter { get; init; }
	public Course Course { get; init; }

	public override string ResponsiveBreakpoint
		=> ResultType.Value switch
		{
			Results.ResultType.BestAverage => "lg",
			Results.ResultType.MostRuns => Filter?.Category != null ? "md" : "lg",
			_ => "xl"
		};
}