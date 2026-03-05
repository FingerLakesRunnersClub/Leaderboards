using FLRC.Leaderboards.Core.Athletes;

namespace FLRC.Leaderboards.Core.Auth;

public interface IWebScorerAuthenticator
{
	Task<Athlete> Login(string email, string password);
}