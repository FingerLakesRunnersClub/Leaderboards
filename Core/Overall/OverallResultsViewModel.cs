using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Overall;

public class OverallResultsViewModel<T> : OverallResultsViewModel
{
	public RankedList<T, Result> RankedResults { get; init; }
}

public class OverallResultsViewModel : DataTableViewModel
{
	public override string Title => ResultType;
	public string ResultType { get; init; }

	public override string ResponsiveBreakpoint
		=> ResultType switch
		{
			"Most Miles" => "md",
			"Members" => "xl",
			_ => "lg"
		};
}