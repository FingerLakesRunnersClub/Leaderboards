using FLRC.Leaderboards.Core.Config;

namespace FLRC.Leaderboards.Core;

public abstract class ViewModel
{
	public abstract string Title { get; }
	public AppConfig Config { get; init; }
}