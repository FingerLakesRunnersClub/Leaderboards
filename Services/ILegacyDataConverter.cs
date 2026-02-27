using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface ILegacyDataConverter
{
	Task<Result[]> ConvertResults(Guid courseID, string source, Core.Results.Result[] legacyResults, DateTime? dateOverride);
	Task<Athlete> GetAthlete(string source, Core.Athletes.Athlete legacyAthlete, DateTime? date = null);
}