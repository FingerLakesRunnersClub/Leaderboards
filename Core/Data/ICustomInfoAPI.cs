namespace FLRC.Leaderboards.Core.Data;

public interface ICustomInfoAPI
{
	Task<IDictionary<string, string>> GetAliases();
	Task<IDictionary<string, IReadOnlyCollection<uint>>> GetGroups();
	Task<IDictionary<uint, DateOnly>> GetPersonalCompletions();
}