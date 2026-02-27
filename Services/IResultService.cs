using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface IResultService
{
	Task<Result> Get(Guid id);
	Task<Result[]> Find(Guid courseID);
	Task Import(Result[] results);
}