using System.Text.Json;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface ICommunityPostAPI
{
	Task<JsonElement[]> GetPosts(ushort id);
	CommunityPost[] ParsePosts(JsonElement[] json);
}