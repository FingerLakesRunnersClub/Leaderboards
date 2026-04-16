using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record SimilarAthlete
{
	public const double Weighting = 0.1;

	public Rank Rank { get; init; }
	public Athlete Athlete { get; init; }
	public Percent Similarity { get; init; }
	public Percent Overlap { get; init; }
	public SpeedComparison FastestPercent { get; init; }
	public SpeedComparison AveragePercent { get; init; }

	public double Score { get; init; }
}