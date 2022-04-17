using System.Text.Json;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Config;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Community;

public class DiscourseAPITests
{
	[Fact]
	public async Task RequestHasRelevantHeadersSet()
	{
		//arrange
		var http = new MockHttpMessageHandler(@"{""post_stream"":{""posts"":[{}]}}");
		var config = Substitute.For<IConfig>();
		config.Features.CommunityStars.Returns(true);
		config.CommunityURL.Returns("https://example.com");
		config.CommunityKey.Returns("abc123");
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		await api.GetPosts(123);

		//assert
		Assert.Equal("https://example.com/t/123.json?include_raw=true", http.Requested.RequestUri?.ToString());
		Assert.Equal("abc123", http.Requested.Headers.First(h => h.Key == "Api-Key").Value.First());
	}

	[Fact]
	public async Task DisabledFeatureReturnsEmpty()
	{
		//arrange
		var http = new MockHttpMessageHandler(@"{""post_stream"":{""posts"":[{}]}}");
		var config = Substitute.For<IConfig>();
		config.CommunityURL.Returns("https://example.com");
		config.CommunityKey.Returns("abc123");
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		var posts = await api.GetPosts(123);

		//assert
		Assert.Empty(posts.EnumerateArray());
	}

	[Fact]
	public void CanParsePosts()
	{
		//arrange
		var http = new MockHttpMessageHandler(@"{""post_stream"":{""posts"":[{}]}}");
		var json = JsonDocument.Parse(@"[{""name"":""User 123"", ""created_at"":""2022-04-07T06:51:23Z"",""raw"":""test 123""}]");
		var config = Substitute.For<IConfig>();
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		var posts = api.ParsePosts(json.RootElement);

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
		var json = JsonDocument.Parse(@"[{""name"":""User 123"", ""created_at"":""2022-04-08T03:51:23Z"",""raw"":""test 123""}]");
		var config = Substitute.For<IConfig>();
		var api = new DiscourseAPI(new HttpClient(http), config);

		//act
		var posts = api.ParsePosts(json.RootElement);

		//act
		Assert.Equal("User 123", posts.First().Name);
		Assert.Equal(new DateTime(2022, 4, 7), posts.First().Date.Date);
		Assert.Equal("test 123", posts.First().Content);
	}
}