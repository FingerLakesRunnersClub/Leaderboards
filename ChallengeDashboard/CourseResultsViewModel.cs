namespace FLRC.ChallengeDashboard;

public class CourseResultsViewModel<T> : CourseResultsViewModel
{
	public RankedList<T> RankedResults { get; init; }
}

public class CourseResultsViewModel : DataTableViewModel
{
	public override string Title => $"{ResultType.Display} — {Course.Name}";

	public FormattedResultType ResultType { get; init; }
	public Category Category { get; init; }
	public Course Course { get; init; }

	public override string ResponsiveBreakpoint
		=> ResultType.Value switch
		{
			ChallengeDashboard.ResultType.BestAverage => "lg",
			ChallengeDashboard.ResultType.MostRuns => Category != null ? "md" : "lg",
			_ => "xl"
		};
}