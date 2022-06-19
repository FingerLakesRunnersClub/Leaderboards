namespace FLRC.Leaderboards.Core.Athletes;

public interface IAliasAPI
{
	Task<IDictionary<string, string>> GetAliases();
}