using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Core.Config;

namespace FLRC.Leaderboards.Services;

public sealed class DiscourseFactory(IContextManager contextManager) : IDiscourseFactory
{
    public async Task<IDiscourseAuthenticator> Authenticator()
    {
        var series = await contextManager.Series();
        return new DiscourseAuthenticator(
            series.Settings.FirstOrDefault(s => s.Key == nameof(IConfig.CommunityURL))?.Value,
            series.Settings.FirstOrDefault(s => s.Key == nameof(IConfig.DiscourseAuthSecret))?.Value
        );
    }
}