using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Athletes;

public sealed class SimilarAthlete
{
	private const double weighting = 0.1;

	public Rank Rank { get; set; }
	public Athlete Athlete { get; }
	public Percent Similarity { get; }
	public Percent Overlap { get; }
	public SpeedComparison FastestPercent { get; }
	public SpeedComparison AveragePercent { get; }

	public double Score { get; }

	public SimilarAthlete(AthleteSummary my, AthleteSummary their)
	{
		var fastestToCompare = GetResultsToCompare(my.Fastest, their.Fastest);
		var avgToCompare = GetResultsToCompare(my.Average, their.Average);
		var matches = fastestToCompare.Count + avgToCompare.Count;
		var fastestDiffTotal = GetDiffTotal(fastestToCompare, their.Fastest) / matches;
		var avgDiffTotal = GetDiffTotal(avgToCompare, their.Average) / matches;
		var total = GetTotal(fastestToCompare, avgToCompare, their);

		Athlete = their.Athlete;
		Similarity = new Percent(total / matches);
		Overlap = new Percent(100.0 * matches / my.TotalResults);
		FastestPercent = fastestToCompare.Count != 0 ? new SpeedComparison(fastestDiffTotal) : null;
		AveragePercent = avgToCompare.Count != 0 ? new SpeedComparison(avgDiffTotal) : null;
		Score = GetScore(Similarity.Value, Overlap.Value);
	}

	private static Dictionary<Course, Ranked<Time>> GetResultsToCompare(IDictionary<Course, Ranked<Time>> mine, IDictionary<Course, Ranked<Time>> theirs)
		=> mine
			.Where(r => r.Value != null && theirs[r.Key] != null)
			.ToDictionary(r => r.Key, r => r.Value);

	private static double GetDiffTotal(IDictionary<Course, Ranked<Time>> my, IDictionary<Course, Ranked<Time>> their)
		=> my.Sum(r => r.Value != null && their[r.Key] != null ? Time.PercentDifference(r.Value.Value, their[r.Key].Value) : 0);

	private static double GetTotal(IDictionary<Course, Ranked<Time>> fastestToCompare, IDictionary<Course, Ranked<Time>> avgToCompare, AthleteSummary their) =>
		fastestToCompare.Sum(r => r.Value != null && their.Fastest[r.Key] != null ? 100 - Time.AbsolutePercentDifference(r.Value.Value, their.Fastest[r.Key].Value) : 0)
		+ avgToCompare.Sum(r => r.Value != null && their.Average[r.Key] != null ? 100 - Time.AbsolutePercentDifference(r.Value.Value, their.Average[r.Key].Value) : 0);

	private static double GetScore(double similarity, double overlap)
	{
		var confidence = similarity * overlap / 100;
		return similarity * (1 - weighting) + confidence * weighting;
	}
}