namespace FLRC.Leaderboards.Core.Groups;

public interface IGroupAPI
{
	Task<IDictionary<string, IEnumerable<uint>>> GetGroups();
}