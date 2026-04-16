using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.ViewModels;

namespace FLRC.Leaderboards.Web.Services;

public static class SimilarAthleteCalculator
{
	public static SimilarAthlete GetSimilarity(AthleteSummary my, AthleteSummary their)
	{
		var fastestToCompare = GetResultsToCompare(my.Fastest, their.Fastest);
		var avgToCompare = GetResultsToCompare(my.Average, their.Average);
		var matches = fastestToCompare.Count + avgToCompare.Count;
		var fastestDiffTotal = GetDiffTotal(fastestToCompare, their.Fastest) / matches;
		var avgDiffTotal = GetDiffTotal(avgToCompare, their.Average) / matches;
		var total = GetTotal(fastestToCompare, avgToCompare, their);

		var similarity = new Percent(total / matches);
		var overlap = new Percent(100.0 * matches / my.TotalResults);
		return new SimilarAthlete
		{
			Athlete = their.Athlete,
			Similarity = similarity,
			Overlap = overlap,
			FastestPercent = fastestToCompare.Count != 0 ? new SpeedComparison(fastestDiffTotal) : null,
			AveragePercent = avgToCompare.Count != 0 ? new SpeedComparison(avgDiffTotal) : null,
			Score = GetScore(similarity.Value, overlap.Value)
		};
	}

	private static Dictionary<Course, Ranked<Time, Result>> GetResultsToCompare(IDictionary<Course, Ranked<Time, Result>> mine, IDictionary<Course, Ranked<Time, Result>> theirs)
		=> mine
			.Where(r => r.Value != null && theirs[r.Key] != null)
			.ToDictionary(r => r.Key, r => r.Value);

	private static double GetDiffTotal(IDictionary<Course, Ranked<Time, Result>> my, IDictionary<Course, Ranked<Time, Result>> their)
		=> my.Sum(r => r.Value != null && their[r.Key] != null ? Time.PercentDifference(r.Value.Value, their[r.Key].Value) : 0);

	private static double GetTotal(IDictionary<Course, Ranked<Time, Result>> fastestToCompare, IDictionary<Course, Ranked<Time, Result>> avgToCompare, AthleteSummary their) =>
		fastestToCompare.Sum(r => r.Value != null && their.Fastest[r.Key] != null ? 100 - Time.AbsolutePercentDifference(r.Value.Value, their.Fastest[r.Key].Value) : 0)
		+ avgToCompare.Sum(r => r.Value != null && their.Average[r.Key] != null ? 100 - Time.AbsolutePercentDifference(r.Value.Value, their.Average[r.Key].Value) : 0);

	private static double GetScore(double similarity, double overlap)
	{
		var confidence = similarity * overlap / 100;
		return similarity * (1 - SimilarAthlete.Weighting) + confidence * SimilarAthlete.Weighting;
	}
}