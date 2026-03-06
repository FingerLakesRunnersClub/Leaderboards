using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface IResultService : IDataService<Result>
{
	Task<Result[]> Find(Guid courseID);
	Task Import(Result[] results);
}