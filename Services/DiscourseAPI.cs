using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public sealed class DiscourseAPI(HttpClient http, IContextManager contextManager) : ICommunityPostAPI, ICommunityUserAPI
{
	public async Task<JsonElement[]> GetPosts(ushort id)
	{
		await SetHttpHeaders();
		var json = await http.GetFromJsonAsync<JsonElement>($"/t/{id}.json?include_raw=true&print=true");
		return json.GetProperty("post_stream").GetProperty("posts").EnumerateArray().ToArray();
	}

	private async Task SetHttpHeaders()
	{
		if (http.BaseAddress is not null && http.DefaultRequestHeaders.Any())
			return;

		var series = await contextManager.Series();
		var settings = series.Setting;

		http.BaseAddress = new Uri(settings[nameof(IConfig.CommunityURL)]);
		http.DefaultRequestHeaders.Add("Api-Key", settings[nameof(IConfig.CommunityKey)]);
	}

	public CommunityPost[] ParsePosts(JsonElement[] json)
		=> json.Select(p => new CommunityPost
		{
			ID = p.GetProperty("user_id").GetUInt16(),
			Name = p.GetProperty("name").GetString() ?? string.Empty,
			Date = p.GetProperty("created_at").GetDateTime().ToLocalTime(),
			Content = p.GetProperty("raw").GetString() ?? string.Empty
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
		await SetHttpHeaders();
		var json = await http.GetFromJsonAsync<JsonElement>($"/groups/{groupID}/members.json?limit={pageSize}&offset={offset}&asc=true");
		return json.GetProperty("members").EnumerateArray().ToArray();
	}

	public async Task<JsonElement> GetGroup(string groupID)
	{
		await SetHttpHeaders();
		var json = await http.GetFromJsonAsync<JsonElement>($"/groups/{groupID}.json");
		return json.GetProperty("group");
	}

	public async Task AddMembers(ushort groupID, string[] usernames)
	{
		var data = new StringContent($$"""{"usernames":"{{string.Join(',', usernames)}}"}""", MediaTypeHeaderValue.Parse("application/json"));
		await SetHttpHeaders();
		await http.PutAsync($"/groups/{groupID}/members.json", data);
	}
}