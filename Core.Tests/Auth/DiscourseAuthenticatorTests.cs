using System.Text;
using FLRC.Leaderboards.Core.Auth;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Auth;

public sealed class DiscourseAuthenticatorTests
{
	[Fact]
	public void LoginURLContainsDiscourseBaseURL()
	{
		//arrange
		var authenticator = new DiscourseAuthenticator("https://example.com/discourse", "secret");

		//act
		var loginURL = authenticator.GetLoginURL("http://localhost");

		//assert
		Assert.StartsWith("https://example.com/discourse/session/sso_provider?sso=", loginURL);
	}

	[Fact]
	public void IsValidWhenDataMatchesSignature()
	{
		//arrange
		const string data = "test";
		const string sig = "0329A06B62CD16B33EB6792BE8C60B158D89A2EE3A876FCE9A881EBB488C0914";

		var authenticator = new DiscourseAuthenticator("https://example.com/discourse", "secret");

		//act
		var valid = authenticator.IsValidResponse(data, sig);

		//assert
		Assert.True(valid);
	}

	[Fact]
	public void IsNotValidWhenDataDoesNotMatchSignature()
	{
		//arrange
		const string data = "hacker";
		const string sig = "0123456789ABCDEF";

		var authenticator = new DiscourseAuthenticator("https://example.com/discourse", "secret");

		//act
		var valid = authenticator.IsValidResponse(data, sig);

		//assert
		Assert.False(valid);
	}

	[Fact]
	public void CanParseResponse()
	{
		//arrange
		var input = Convert.ToBase64String("field=value"u8.ToArray());

		var authenticator = new DiscourseAuthenticator("https://example.com/discourse", "secret");

		//act
		var data = authenticator.ParseResponse(input);

		//asser
		Assert.Equal("value", data["field"]);
	}
}