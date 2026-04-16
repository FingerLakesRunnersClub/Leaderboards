using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web;

public static class OverallExtensions
{
	extension<T>(IGrouping<Athlete, Ranked<T, Result>> result)
	{
		public double AvgAgeGrade()
		{
			var results = result.Where(r => r.AgeGrade is not null).ToArray();

			return results.Length > 0
				? results.Average(r => r.AgeGrade.Value)
				: 0;
		}
	}
}