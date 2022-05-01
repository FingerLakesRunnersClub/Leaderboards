using System.Text.Json;
using FLRC.Leaderboards.Core.Config;

namespace FLRC.Leaderboards.Core.Community;

public class DiscourseAPI : ICommunityAPI
{
	private const byte PageSize = 20;

	private readonly HttpClient _http;

	public DiscourseAPI(HttpClient http, IConfig config)
	{
		if (!config.Features.CommunityStars)
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

	public async Task<IReadOnlyCollection<JsonElement>> GetPosts(ushort id)
	{
		if (_http == null)
		{
			return Array.Empty<JsonElement>();
		}

		var page1 = await GetResponse(id, 1);
		var postCount = page1.GetProperty("posts_count").GetUInt16();
		var pageCount = (byte) Math.Ceiling(postCount * 1.0 / PageSize);
		var pageTasks = pageCount > 1
			? Enumerable.Range(2, pageCount - 1).Select(p => GetResponse(id, (byte) p))
			: Enumerable.Empty<Task<JsonElement>>();
		var pages = await Task.WhenAll(pageTasks);
		return page1.GetProperty("post_stream").GetProperty("posts").EnumerateArray()
			.Union(pages.SelectMany(p => p.GetProperty("post_stream").GetProperty("posts").EnumerateArray()))
			.ToArray();
	}

	private async Task<JsonElement> GetResponse(ushort id, byte page)
	{
		var response = await _http.GetStreamAsync($"/t/{id}.json?page={page}&include_raw=true");
		var json = await JsonDocument.ParseAsync(response);
		return json.RootElement;
	}

	public IReadOnlyCollection<Post> ParsePosts(IReadOnlyCollection<JsonElement> json)
		=> json.Select(p => new Post
		{
			Name = p.GetProperty("name").GetString(),
			Date = p.GetProperty("created_at").GetDateTime().ToLocalTime(),
			Content = p.GetProperty("raw").GetString()
		}).ToArray();
}