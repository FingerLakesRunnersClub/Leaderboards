using FLRC.Leaderboards.Core.Auth;

namespace FLRC.Leaderboards.Services;

public interface IDiscourseFactory
{
    Task<IDiscourseAuthenticator> Authenticator();
}