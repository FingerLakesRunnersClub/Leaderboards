using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Overall;

public static class OverallExtensions
{
	public static double AvgAgeGrade<T>(this IGrouping<Athlete, Ranked<T>> result)
	{
		var results = result.Where(r => r.AgeGrade is not null).ToArray();

		return results.Length > 0
			? results.Average(r => r.AgeGrade.Value)
			: 0;
	}
}