using FLRC.Leaderboards.Data.Models;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public interface IImportManager
{
	Task<Athlete[]> ImportAthletes(string source, uint externalID);
	Task<Result[]> ImportResults(Course course, string source, uint externalID, DateTime? dateOverride = null);
	string[] Sources { get; }
}