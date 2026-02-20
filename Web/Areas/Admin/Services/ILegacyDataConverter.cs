using FLRC.Leaderboards.Data.Models;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public interface ILegacyDataConverter
{
	Task<Result[]> ConvertResults(Guid courseID, string source, Core.Results.Result[] legacyResults, DateTime? dateOverride);
	Task<Athlete> GetAthlete(string source, Core.Athletes.Athlete legacyAthlete);
}