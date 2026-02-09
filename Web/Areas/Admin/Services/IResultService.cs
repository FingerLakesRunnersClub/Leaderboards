using FLRC.Leaderboards.Data.Models;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public interface IResultService
{
	Task<Result> Get(Guid id);
	Task<Result[]> Find(Guid courseID);
	Task Import(Result[] results);
}