using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface IResultService : IDataService<Result>
{
	Task<Result[]> Find(Guid courseID);
	Task<Result[]> Find(Iteration iteration);
	Task Import(Result[] results);
}