using FLRC.Leaderboards.Data.Models;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public interface IIterationService
{
	Task<Iteration[]> GetAllIterations();
	Task<Iteration> GetIteration(Guid id);
	Task AddIteration(Guid id, Iteration iteration);
	Task UpdateIteration(Iteration iteration, Iteration updated);

	Task UpdateRaces(Iteration iteration, Race[] races);
	Task UpdateRegistrations(Iteration iteration, Athlete[] athletes);
}