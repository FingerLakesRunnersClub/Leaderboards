namespace FLRC.Leaderboards.Core;

public interface IGroupAPI
{
	Task<IDictionary<string, IEnumerable<uint>>> GetGroups();
}