using System.Text.Json;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Config;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Community;

public sealed class DiscourseAPITests
{
	[Fact]
	public void CanCreateWithoutURL()
	{
		//arrange
		var http = new MockHttpMessageHandler(@"{""post_stream"":{""posts"":[{}]},""posts_count"":0}");
		var config = Substitute.For<IConfig>();

		//act
		var api = new DiscourseAPI(new HttpClient(http), config);

		//assert
		Assert.NotNull(api);
	}
	[Fact]
	public async Task RequestHasRelevantHeadersSet()
	{
		//arrange
		var http = new MockHttpMessageHandler(@"{""post_stream"":{""posts"":[{}]},""posts_count"":0}");
		var config = Substitute.For<IConfig>();
		config.Features.CommunityStars.Returns(true);
		config.CommunityURL.Returns("https://example.com");
		config.CommunityKey.Returns("abc123");
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		await api.GetPosts(123);

		//assert
		var queryString = http.LastRequested.RequestUri!.Query;
		Assert.Contains("page=1", queryString);
		Assert.Contains("include_raw=true", queryString);
		Assert.Equal("abc123", http.LastRequested.Headers.First(h => h.Key == "Api-Key").Value.First());
	}

	[Fact]
	public async Task CanGetPostsFromSinglePage()
	{
		//arrange
		var http = new MockHttpMessageHandler(@"{""post_stream"":{""posts"":[{}]},""posts_count"":20}");
		var config = Substitute.For<IConfig>();
		config.Features.CommunityStars.Returns(true);
		config.CommunityURL.Returns("https://example.com");
		config.CommunityKey.Returns("abc123");
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		var posts = await api.GetPosts(123);

		//assert
		Assert.Single(posts);
	}

	[Fact]
	public async Task CanGetPostsFromMultiplePages()
	{
		//arrange
		var http = new MockHttpMessageHandler(@"{""post_stream"":{""posts"":[{}]},""posts_count"":21}");
		var config = Substitute.For<IConfig>();
		config.Features.CommunityStars.Returns(true);
		config.CommunityURL.Returns("https://example.com");
		config.CommunityKey.Returns("abc123");
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		var posts = await api.GetPosts(123);

		//assert
		Assert.Equal(2, posts.Count);
	}

	[Fact]
	public void CanParsePosts()
	{
		//arrange
		var http = new MockHttpMessageHandler(@"{""post_stream"":{""posts"":[{}]}}");
		var json = JsonDocument.Parse(@"{""name"":""User 123"", ""created_at"":""2022-04-07T06:51:23Z"",""raw"":""test 123""}");
		var config = Substitute.For<IConfig>();
		config.CommunityURL.Returns("https://example.com");
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		var posts = api.ParsePosts(new[] { json.RootElement });

		//act
		Assert.Equal("User 123", posts.First().Name);
		Assert.Equal(new DateTime(2022, 4, 7), posts.First().Date.Date);
		Assert.Equal("test 123", posts.First().Content);
	}

	[Fact]
	public void ParsePostsConvertsTimeZone()
	{
		//arrange
		var http = new MockHttpMessageHandler(@"{""post_stream"":{""posts"":[{}]}}");
		var json = JsonDocument.Parse(@"{""name"":""User 123"", ""created_at"":""2022-04-08T03:51:23Z"",""raw"":""test 123""}");
		var config = Substitute.For<IConfig>();
		config.CommunityURL.Returns("https://example.com");
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		var posts = api.ParsePosts(new[] { json.RootElement });

		//act
		Assert.Equal("User 123", posts.First().Name);
		Assert.Equal(new DateTime(2022, 4, 7), posts.First().Date.Date);
		Assert.Equal("test 123", posts.First().Content);
	}

	[Fact]
	public async Task CanGetUsers()
	{
		//arrange
		var http = new MockHttpMessageHandler(@"{""members"":[{""id"":123,""name"": ""Steve Desmond""}]}");
		var config = Substitute.For<IConfig>();
		config.CommunityURL.Returns("https://example.com");
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		var json = await api.GetUsers();

		//assert
		Assert.Equal(123, json.First().GetProperty("id").GetByte());
	}

	[Fact]
	public async Task CanGetGroupMembers()
	{
		//arrange
		var http = new MockHttpMessageHandler(@"{""members"":[{""id"":123,""name"": ""Steve Desmond""}]}");
		var config = Substitute.For<IConfig>();
		config.CommunityURL.Returns("https://example.com");
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		var json = await api.GetMembers("test");

		//assert
		Assert.Equal(123, json.First().GetProperty("id").GetByte());
	}

	[Fact]
	public async Task SmallNumberOfGroupMembersOnlyMakesOneRequest()
	{
		//arrange
		var http = new MockHttpMessageHandler(@"{""members"":[{""id"":123,""name"": ""Steve Desmond""}]}");
		var config = Substitute.For<IConfig>();
		config.CommunityURL.Returns("https://example.com");
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		await api.GetMembers("test");

		//assert
		Assert.Single(http.Requests);
	}

	[Fact]
	public async Task LargeNumberOfGroupMembersMakesMultipleRequests()
	{
		//arrange
		var data = new { members = Enumerable.Range(1, 1000).Select(n => new { id = n, name = "test" }) };
		var http = new MockHttpMessageHandler(JsonSerializer.Serialize(data));
		var config = Substitute.For<IConfig>();
		config.CommunityURL.Returns("https://example.com");
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		await api.GetMembers("test");

		//assert
		Assert.Equal(2, http.Requests.Count);
	}

	[Fact]
	public async Task CanGetGroupInfo()
	{
		//arrange
		var http = new MockHttpMessageHandler(@"{""group"":{""id"":123,""name"": ""test""}}");
		var config = Substitute.For<IConfig>();
		config.CommunityURL.Returns("https://example.com");
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		var json = await api.GetGroup("test");

		//assert
		Assert.Equal(123, json.GetProperty("id").GetByte());
	}

	[Fact]
	public async Task CanAddMembersToGroup()
	{
		//arrange
		var http = new MockHttpMessageHandler(string.Empty);
		var config = Substitute.For<IConfig>();
		config.CommunityURL.Returns("https://example.com");
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		await api.AddMembers(123, new[] { "user1", "user2" });

		//assert
		Assert.Equal("/groups/123/members.json", http.LastRequested.RequestUri!.AbsolutePath);
		Assert.Equal(@"{""usernames"":""user1,user2""}", await http.LastRequested.Content!.ReadAsStringAsync());
	}
}