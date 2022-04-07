using System.Text.Json;
using FLRC.Leaderboards.Core.Config;

namespace FLRC.Leaderboards.Core.Community;

public class DiscourseAPI : ICommunityAPI
{
	private readonly HttpClient _http;

	public DiscourseAPI(HttpClient http, IConfig config)
	{
		if (!config.Features.CommunityPoints)
		{
			return;
		}

		_http = http;
		_http.BaseAddress = new Uri(config.CommunityURL);
		if (!string.IsNullOrWhiteSpace(config.CommunityKey))
		{
			_http.DefaultRequestHeaders.Add("Api-Key", config.CommunityKey);
		}
	}

	public async Task<JsonElement> GetPosts(ushort id)
	{
		if (_http == null || id == 0)
		{
			return JsonDocument.Parse("[]").RootElement;
		}

		var response = await _http.GetStreamAsync($"/t/{id}.json?include_raw=true");
		var json = await JsonDocument.ParseAsync(response);
		return json.RootElement.GetProperty("post_stream").GetProperty("posts");
	}
}