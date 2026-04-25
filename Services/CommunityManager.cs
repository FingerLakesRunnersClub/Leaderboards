using System.Text.Json;
using FLRC.Leaderboards.Core.Community;

namespace FLRC.Leaderboards.Services;

public interface ICommunityManager
{
	Task<User[]> GetCommunityUsers();
	Task<User[]> GetCommunityGroupMembers(string groupID);
	Task AddCommunityGroupMembers(IDictionary<string, string[]> groupAdditions);
}

public class CommunityManager(ICommunityAPI communityAPI) : ICommunityManager
{
	public async Task<User[]> GetCommunityUsers()
	{
		var users = await communityAPI.GetUsers();
		return users.Select(ParseUser).ToArray();
	}

	public async Task<User[]> GetCommunityGroupMembers(string groupID)
	{
		var users = await communityAPI.GetMembers(groupID);
		return users.Select(ParseUser).ToArray();
	}

	public async Task AddCommunityGroupMembers(IDictionary<string, string[]> groupAdditions)
	{
		foreach (var group in groupAdditions)
		{
			var info = await communityAPI.GetGroup(group.Key);
			var id = info.GetProperty("id").GetUInt16();
			await communityAPI.AddMembers(id, group.Value);
		}
	}

	private static User ParseUser(JsonElement json)
		=> new()
		{
			ID = json.GetProperty("id").GetUInt16(),
			Name = json.GetProperty("name").GetString(),
			Username = json.GetProperty("username").GetString()
		};
}