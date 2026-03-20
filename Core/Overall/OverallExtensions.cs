using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Overall;

public static class OverallExtensions
{
	public static double AvgAgeGrade<T>(this IGrouping<Athlete, Ranked<T, Result>> result)
	{
		var results = result.Where(r => r.AgeGrade is not null).ToArray();

		return results.Length > 0
			? results.Average(r => r.AgeGrade.Value)
			: 0;
	}
}