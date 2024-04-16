using System.Net.Http.Headers;
using System.Text.Json;
using FLRC.Leaderboards.Core.Config;

namespace FLRC.Leaderboards.Core.Community;

public sealed class DiscourseAPI : ICommunityAPI
{
	private const byte PageSize = 20;

	private readonly HttpClient _http;

	public DiscourseAPI(HttpClient http, IConfig config)
	{
		if (string.IsNullOrWhiteSpace(config.CommunityURL))
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

	public async Task<JsonElement[]> GetPosts(ushort id)
	{
		var page1 = await GetPostResponse(id, 1);
		var postCount = page1.GetProperty("posts_count").GetUInt16();
		var pageCount = (byte) Math.Ceiling(postCount * 1.0 / PageSize);
		var pageTasks = pageCount > 1
			? Enumerable.Range(2, pageCount - 1).Select(p => GetPostResponse(id, (byte) p))
			: Enumerable.Empty<Task<JsonElement>>();
		var pages = await Task.WhenAll(pageTasks);
		return page1.GetProperty("post_stream").GetProperty("posts").EnumerateArray()
			.Union(pages.SelectMany(p => p.GetProperty("post_stream").GetProperty("posts").EnumerateArray()))
			.ToArray();
	}

	private async Task<JsonElement> GetPostResponse(ushort id, byte page)
	{
		var response = await _http.GetStreamAsync($"/t/{id}.json?page={page}&include_raw=true");
		var json = await JsonDocument.ParseAsync(response);
		return json.RootElement;
	}

	public Post[] ParsePosts(JsonElement[] json)
		=> json.Select(p => new Post
		{
			Name = p.GetProperty("name").GetString(),
			Date = p.GetProperty("created_at").GetDateTime().ToLocalTime(),
			Content = p.GetProperty("raw").GetString()
		}).ToArray();

	public async Task<JsonElement[]> GetUsers()
		=> await GetMembers("trust_level_0");

	public async Task<JsonElement[]> GetMembers(string groupID)
	{
		const int pageSize = 1000;

		var members = await GetMembers(groupID, pageSize, 1);
		if (members.Length < pageSize)
			return members;

		var moreMembers = await GetMembers(groupID, pageSize, 2);
		return members.Concat(moreMembers).ToArray();
	}

	private async Task<JsonElement[]> GetMembers(string groupID, int pageSize, int pageNumber)
	{
		var offset = pageSize * (pageNumber - 1);
		var response = await _http.GetStreamAsync($"/groups/{groupID}/members.json?limit={pageSize}&offset={offset}&asc=true");
		var json = await JsonDocument.ParseAsync(response);
		return json.RootElement.GetProperty("members").EnumerateArray().ToArray();
	}

	public async Task<JsonElement> GetGroup(string groupID)
	{
		var response = await _http.GetStreamAsync($"/groups/{groupID}.json");
		var json = await JsonDocument.ParseAsync(response);
		return json.RootElement.GetProperty("group");
	}

	public async Task AddMembers(ushort groupID, string[] usernames)
	{
		var data = new StringContent($$"""{"usernames":"{{string.Join(',', usernames)}}"}""", MediaTypeHeaderValue.Parse("application/json"));
		await _http.PutAsync($"/groups/{groupID}/members.json", data);
	}
}