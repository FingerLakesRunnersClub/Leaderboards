using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Groups;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Groups;

public class GroupAPITests
{
	[Fact]
	public async Task CanGetGroupMemberIDs()
	{
		//arrange
		const string data = @"{ ""Test 1"": [ 123, 234 ], ""Test 2"": [ 234, 345 ] }";
		var http = new MockHttpMessageHandler(data);
		var config = Substitute.For<IConfig>();
		config.GroupAPI.Returns("http://localhost");
		var api = new GroupAPI(new HttpClient(http), config);

		//act
		var groups = await api.GetGroups();

		//assert
		var members1 = groups["Test 1"].ToArray();
		Assert.Equal((uint)123, members1[0]);
		Assert.Equal((uint)234, members1[1]);

		var members2 = groups["Test 2"].ToArray();
		Assert.Equal((uint)234, members2[0]);
		Assert.Equal((uint)345, members2[1]);
	}
}