using FLRC.Leaderboards.Core.Community;

namespace FLRC.Leaderboards.Services;

public interface ICommunityManager
{
	Task<User[]> GetCommunityUsers();
	Task<User[]> GetCommunityGroupMembers(string groupID);
	Task AddCommunityGroupMembers(IDictionary<string, string[]> groupAdditions);
}