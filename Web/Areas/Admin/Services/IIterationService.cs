using FLRC.Leaderboards.Data.Models;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public interface IIterationService
{
	Task<Iteration[]> GetAllIterations();
	Task<Iteration> GetIteration(Guid id);
	Task AddIteration(Guid id, Iteration iteration, Guid[] races);
	Task UpdateIteration(Iteration iteration, Iteration updated, Guid[] races);
}