using System.Text.Json;

namespace FLRC.Leaderboards.Core.Community;

public interface ICommunityAPI
{
	Task<IReadOnlyCollection<JsonElement>> GetPosts(ushort id);
	IReadOnlyCollection<Post> ParsePosts(IReadOnlyCollection<JsonElement> json);

	Task<IReadOnlyCollection<JsonElement>> GetUsers();
	Task<JsonElement> GetGroup(string groupID);
	Task<IReadOnlyCollection<JsonElement>> GetMembers(string groupID);
	Task AddMembers(ushort groupID, IReadOnlyCollection<string> usernames);
}