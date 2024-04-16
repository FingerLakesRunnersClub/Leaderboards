using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FLRC.Leaderboards.Core.Config;

namespace FLRC.Leaderboards.Core.Community;

public sealed class DiscourseAPI : ICommunityAPI
{
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
		var json = await _http.GetFromJsonAsync<JsonElement>($"/t/{id}.json?include_raw=true&print=true");
		return json.GetProperty("post_stream").GetProperty("posts").EnumerateArray().ToArray();
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
		var json = await _http.GetFromJsonAsync<JsonElement>($"/groups/{groupID}/members.json?limit={pageSize}&offset={offset}&asc=true");
		return json.GetProperty("members").EnumerateArray().ToArray();
	}

	public async Task<JsonElement> GetGroup(string groupID)
	{
		var json = await _http.GetFromJsonAsync<JsonElement>($"/groups/{groupID}.json");
		return json.GetProperty("group");
	}

	public async Task AddMembers(ushort groupID, string[] usernames)
	{
		var data = new StringContent($$"""{"usernames":"{{string.Join(',', usernames)}}"}""", MediaTypeHeaderValue.Parse("application/json"));
		await _http.PutAsync($"/groups/{groupID}/members.json", data);
	}
}