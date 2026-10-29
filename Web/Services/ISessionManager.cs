using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.Services;

public interface ISessionManager
{
    Task<Athlete> GetCurrentAthlete();
}