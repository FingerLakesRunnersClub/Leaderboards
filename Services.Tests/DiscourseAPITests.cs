using System.Collections;
using System.Text.Json;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Model;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public sealed class DiscourseAPITests
{
	[Fact]
	public void CanCreateWithoutURL()
	{
		//arrange
		var http = new MockHttpMessageHandler("""{"post_stream":{"posts":[{}]},"posts_count":0}""");
		var contextManager = Substitute.For<IContextManager>();

		var id = Guid.NewGuid();
		var series = new Series
		{
			ID = id,
			Settings =
			[
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityURL), Value = "https://example.com" },
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityKey), Value = "abc123" },
			]
		};
		contextManager.Series().Returns(series);

		//act
		var api = new DiscourseAPI(new HttpClient(http), contextManager);

		//assert
		Assert.NotNull(api);
	}

	[Fact]
	public async Task RequestHasRelevantHeadersSet()
	{
		//arrange
		var http = new MockHttpMessageHandler("""{"post_stream":{"posts":[{}]},"posts_count":0}""");
		var contextManager = Substitute.For<IContextManager>();

		var id = Guid.NewGuid();
		var series = new Series
		{
			ID = id,
			Settings =
			[
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityURL), Value = "https://example.com" },
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityKey), Value = "abc123" },
			]
		};
		contextManager.Series().Returns(series);

		var api = new DiscourseAPI(new HttpClient(http), contextManager);

		//act
		await api.GetPosts(123);

		//assert
		var queryString = http.LastRequested.RequestUri!.Query;
		Assert.Contains("include_raw=true", queryString);
		Assert.Contains("print=true", queryString);
		Assert.Equal("abc123", http.LastRequested.Headers.First(h => h.Key == "Api-Key").Value.First());
	}

	[Fact]
	public async Task CanGetPostsFromSinglePage()
	{
		//arrange
		var http = new MockHttpMessageHandler("""{"post_stream":{"posts":[{}]},"posts_count":20}""");
		var contextManager = Substitute.For<IContextManager>();

		var id = Guid.NewGuid();
		var series = new Series
		{
			ID = id,
			Settings =
			[
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityURL), Value = "https://example.com" },
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityKey), Value = "abc123" },
			]
		};
		contextManager.Series().Returns(series);

		var api = new DiscourseAPI(new HttpClient(http), contextManager);

		//act
		var posts = await api.GetPosts(123);

		//assert
		Assert.Single((IEnumerable)posts);
	}

	[Fact]
	public void CanParsePosts()
	{
		//arrange
		var http = new MockHttpMessageHandler("""{"post_stream":{"posts":[{}]}}""");
		var json = JsonDocument.Parse("""{"name":"User 123", "created_at":"2022-04-07T06:51:23Z","raw":"test 123"}""");
		var contextManager = Substitute.For<IContextManager>();

		var id = Guid.NewGuid();
		var series = new Series
		{
			ID = id,
			Settings =
			[
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityURL), Value = "https://example.com" },
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityKey), Value = "abc123" },
			]
		};
		contextManager.Series().Returns(series);

		var api = new DiscourseAPI(new HttpClient(http), contextManager);

		//act
		var posts = api.ParsePosts([json.RootElement]);

		//act
		Assert.Equal("User 123", posts.First().Name);
		Assert.Equal(new DateTime(2022, 4, 7), posts.First().Date.Date);
		Assert.Equal("test 123", posts.First().Content);
	}

	[Fact]
	public void ParsePostsConvertsTimeZone()
	{
		//arrange
		var http = new MockHttpMessageHandler("""{"post_stream":{"posts":[{}]}}""");
		var json = JsonDocument.Parse("""{"name":"User 123", "created_at":"2022-04-08T03:51:23Z","raw":"test 123"}""");
		var contextManager = Substitute.For<IContextManager>();

		var id = Guid.NewGuid();
		var series = new Series
		{
			ID = id,
			Settings =
			[
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityURL), Value = "https://example.com" },
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityKey), Value = "abc123" },
			]
		};
		contextManager.Series().Returns(series);

		var api = new DiscourseAPI(new HttpClient(http), contextManager);

		//act
		var posts = api.ParsePosts([json.RootElement]);

		//act
		Assert.Equal("User 123", posts.First().Name);
		Assert.Equal(new DateTime(2022, 4, 7), posts.First().Date.Date);
		Assert.Equal("test 123", posts.First().Content);
	}

	[Fact]
	public async Task CanGetUsers()
	{
		//arrange
		var http = new MockHttpMessageHandler("""{"members":[{"id":123,"name": "Steve Desmond"}]}""");
		var contextManager = Substitute.For<IContextManager>();

		var id = Guid.NewGuid();
		var series = new Series
		{
			ID = id,
			Settings =
			[
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityURL), Value = "https://example.com" },
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityKey), Value = "abc123" },
			]
		};
		contextManager.Series().Returns(series);

		var api = new DiscourseAPI(new HttpClient(http), contextManager);

		//act
		var json = await api.GetUsers();

		//assert
		Assert.Equal(123, json.First().GetProperty("id").GetByte());
	}

	[Fact]
	public async Task CanGetGroupMembers()
	{
		//arrange
		var http = new MockHttpMessageHandler("""{"members":[{"id":123,"name": "Steve Desmond"}]}""");
		var contextManager = Substitute.For<IContextManager>();

		var id = Guid.NewGuid();
		var series = new Series
		{
			ID = id,
			Settings =
			[
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityURL), Value = "https://example.com" },
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityKey), Value = "abc123" },
			]
		};
		contextManager.Series().Returns(series);

		var api = new DiscourseAPI(new HttpClient(http), contextManager);

		//act
		var json = await api.GetMembers("test");

		//assert
		Assert.Equal(123, json.First().GetProperty("id").GetByte());
	}

	[Fact]
	public async Task SmallNumberOfGroupMembersOnlyMakesOneRequest()
	{
		//arrange
		var http = new MockHttpMessageHandler("""{"members":[{"id":123,"name": "Steve Desmond"}]}""");
		var contextManager = Substitute.For<IContextManager>();

		var id = Guid.NewGuid();
		var series = new Series
		{
			ID = id,
			Settings =
			[
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityURL), Value = "https://example.com" },
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityKey), Value = "abc123" },
			]
		};
		contextManager.Series().Returns(series);

		var api = new DiscourseAPI(new HttpClient(http), contextManager);

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
		var contextManager = Substitute.For<IContextManager>();

		var id = Guid.NewGuid();
		var series = new Series
		{
			ID = id,
			Settings =
			[
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityURL), Value = "https://example.com" },
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityKey), Value = "abc123" },
			]
		};
		contextManager.Series().Returns(series);

		var api = new DiscourseAPI(new HttpClient(http), contextManager);

		//act
		await api.GetMembers("test");

		//assert
		Assert.Equal(2, http.Requests.Count);
	}

	[Fact]
	public async Task CanGetGroupInfo()
	{
		//arrange
		var http = new MockHttpMessageHandler("""{"group":{"id":123,"name": "test"}}""");
		var contextManager = Substitute.For<IContextManager>();

		var id = Guid.NewGuid();
		var series = new Series
		{
			ID = id,
			Settings =
			[
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityURL), Value = "https://example.com" },
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityKey), Value = "abc123" },
			]
		};
		contextManager.Series().Returns(series);

		var api = new DiscourseAPI(new HttpClient(http), contextManager);

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
		var contextManager = Substitute.For<IContextManager>();

		var id = Guid.NewGuid();
		var series = new Series
		{
			ID = id,
			Settings =
			[
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityURL), Value = "https://example.com" },
				new Setting { SeriesID = id, Key = nameof(IConfig.CommunityKey), Value = "abc123" },
			]
		};
		contextManager.Series().Returns(series);

		var api = new DiscourseAPI(new HttpClient(http), contextManager);

		//act
		await api.AddMembers(123, ["user1", "user2"]);

		//assert
		Assert.Equal("/groups/123/members.json", http.LastRequested.RequestUri!.AbsolutePath);
		Assert.Equal("""{"usernames":"user1,user2"}""", await http.LastRequested.Content!.ReadAsStringAsync());
	}
}