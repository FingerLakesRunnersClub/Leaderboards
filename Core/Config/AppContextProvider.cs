namespace FLRC.Leaderboards.Core.Config;

public sealed class AppContextProvider(string app) : IContextProvider
{
	public string App => app;
	public override string ToString() => app;
}