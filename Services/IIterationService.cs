using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface IIterationService : IDataService<Iteration>
{
	Task<Iteration?> Current(Guid seriesID);
	Task<Iteration?> MostRecent(Guid seriesID);

	Task UpdateRaces(Iteration iteration, Race[] races);
	Task UpdateRegistrations(Iteration iteration, Athlete[] athletes);
}