namespace FLRC.Leaderboards.Core.Groups;

public interface IGroupAPI
{
	Task<IDictionary<string, IReadOnlyCollection<uint>>> GetGroups();
}