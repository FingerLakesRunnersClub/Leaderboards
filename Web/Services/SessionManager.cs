using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;

namespace FLRC.Leaderboards.Web.Services;

public class SessionManager(IAuthService authService, IAthleteService athleteService) : ISessionManager
{
    public async Task<Athlete> GetCurrentAthlete()
    {
        if (!authService.IsLoggedIn())
            return null;

        var user = authService.GetCurrentUser();
        var claims = user.ClaimDictionary;

        return await athleteService.Find(LinkedAccount.Keys.Discourse, claims["external_id"])
               ?? await athleteService.Find(LinkedAccount.Keys.Email, claims["email"]);
    }
}