using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.ViewModels;

namespace FLRC.Leaderboards.Web.Services;

public interface IAthleteSummaryCalculator
{
	Task<AthleteSummary> GetSummary(Athlete athlete, Iteration iteration);
	Task<SimilarAthlete[]> SimilarAthletes(AthleteSummary summary);
}