using FLRC.Leaderboards.Core.Auth;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Auth;

public sealed class WebScorerAuthenticatorTests
{
	[Fact]
	public async Task CanGetLoginResponseFromWebScorer()
	{
		//arrange
		var handler = new MockHttpMessageHandler(@"{""UserId"":456}");
		var http = new HttpClient(handler);
		var auth = new WebScorerAuthenticator(http);

		//act
		var id = await auth.Login("test", "123");

		//assert
		Assert.Equal((uint)456, id);
	}
}