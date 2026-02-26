namespace FLRC.Leaderboards.Core.Auth;

public interface IWebScorerAuthenticator
{
	Task<uint> Login(string email, string password);
}