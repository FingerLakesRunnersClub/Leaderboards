using FLRC.Leaderboards.Core.Config;

namespace FLRC.Leaderboards.Core;

public abstract class ViewModel
{
	public abstract string Title { get; }
	public IConfig Config { get; init; }
}

public sealed class ViewModel<T>(string title, T data) : ViewModel
{
	public override string Title => title;

	public T Data => data;
}