using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface IContextManager
{
    Task<Series> Series();
}