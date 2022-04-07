using System.Text.Json;

namespace FLRC.Leaderboards.Core.Community;

public interface ICommunityAPI
{
	Task<JsonElement> GetPosts(ushort id);
}