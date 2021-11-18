namespace FLRC.ChallengeDashboard;

public interface IGroupAPI
{
	Task<IDictionary<string, IEnumerable<uint>>> GetGroups();
}