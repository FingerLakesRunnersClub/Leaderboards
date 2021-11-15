using System.Linq;

namespace FLRC.ChallengeDashboard;

public class SimilarAthlete
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
		var fastestToCompare = my.Fastest.Where(r => r.Value != null && their.Fastest[r.Key] != null).ToList();
		var avgToCompare = my.Average.Where(r => r.Value != null && their.Average[r.Key] != null).ToList();
		var totalMatches = fastestToCompare.Count + avgToCompare.Count;

		var fastestDiffTotal = fastestToCompare.Sum(r => r.Value != null && their.Fastest[r.Key] != null ? Time.PercentDifference(r.Value.Value, their.Fastest[r.Key].Value) : 0) / totalMatches;
		var avgDiffTotal = avgToCompare.Sum(r => r.Value != null && their.Average[r.Key] != null ? Time.PercentDifference(r.Value.Value, their.Average[r.Key].Value) : 0) / totalMatches;

		var score = fastestToCompare.Sum(r => r.Value != null && their.Fastest[r.Key] != null ? 100 - Time.AbsolutePercentDifference(r.Value.Value, their.Fastest[r.Key].Value) : 0)
			+ avgToCompare.Sum(r => r.Value != null && their.Average[r.Key] != null ? 100 - Time.AbsolutePercentDifference(r.Value.Value, their.Average[r.Key].Value) : 0);

		Athlete = their.Athlete;
		Similarity = new Percent(score / totalMatches);
		Overlap = new Percent(100.0 * totalMatches / my.TotalResults);
		FastestPercent = fastestToCompare.Any() ? new SpeedComparison(fastestDiffTotal) : null;
		AveragePercent = avgToCompare.Any() ? new SpeedComparison(avgDiffTotal) : null;

		var confidence = Similarity.Value * Overlap.Value / 100;
		Score = Similarity.Value * (1 - weighting) + confidence * weighting;
	}
}
