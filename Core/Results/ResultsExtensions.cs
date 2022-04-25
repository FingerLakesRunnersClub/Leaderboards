namespace FLRC.Leaderboards.Core.Results;

public static class ResultsExtensions
{
	public static IEnumerable<Result> Filter(this IEnumerable<Result> results, Filter filter)
		=> results.Where(r => filter == null || filter.IsMatch(r));
}