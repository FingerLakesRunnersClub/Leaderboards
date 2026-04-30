using System.Text.Json;

namespace FLRC.Leaderboards.Core.Community;

public interface ICommunityUserAPI
{
	Task<JsonElement[]> GetUsers();
	Task<JsonElement> GetGroup(string groupID);
	Task<JsonElement[]> GetMembers(string groupID);
	Task AddMembers(ushort groupID, string[] usernames);
}