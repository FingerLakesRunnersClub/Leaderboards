using System.Text.Json;

namespace FLRC.Leaderboards.Core.Community;

public interface ICommunityAPI
{
	Task<IReadOnlyCollection<JsonElement>> GetPosts(ushort id);
	IReadOnlyCollection<Post> ParsePosts(IReadOnlyCollection<JsonElement> posts);
}