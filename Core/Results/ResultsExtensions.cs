namespace FLRC.Leaderboards.Core.Results;

public static class ResultsExtensions
{
	public static Result[] Filter(this Result[] results, Filter filter)
		=> results.Where(r => filter == null || filter.IsMatch(r)).ToArray();
}