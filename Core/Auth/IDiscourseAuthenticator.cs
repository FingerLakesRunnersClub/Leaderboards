namespace FLRC.Leaderboards.Core.Auth;

public interface IDiscourseAuthenticator
{
	string GetLoginURL(string currentHost);
	bool IsValidResponse(string sso, string sig);
	IDictionary<string, string> ParseResponse(string sso);
}