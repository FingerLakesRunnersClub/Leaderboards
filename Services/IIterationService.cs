using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface IIterationService
{
	Task<Iteration[]> GetAllIterations();
	Task<Iteration> GetIteration(Guid id);
	Task AddIteration(Guid id, Iteration iteration);
	Task UpdateIteration(Iteration iteration, Iteration updated);

	Task UpdateRaces(Iteration iteration, Race[] races);
	Task UpdateRegistrations(Iteration iteration, Athlete[] athletes);
}