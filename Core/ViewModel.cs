namespace FLRC.Leaderboards.Core;

public abstract class ViewModel
{
	public abstract string Title { get; }
	public Config Config { get; init; }
}