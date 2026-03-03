using System.Security.Claims;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class UserExtensionsTests
{
	[Fact]
	public void CanGetDictionaryOfClaimsFromIdentity()
	{
		//arrange
		var user = new ClaimsIdentity([new Claim("test", "value")]);

		//act
		var claims = user.ClaimDictionary;

		//assert
		Assert.Equal("value", claims["test"]);
	}

	[Fact]
	public void CanGetDictionaryOfClaimsFromPrincipal()
	{
		//arrange
		var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("test", "value")]));

		//act
		var claims = user.ClaimDictionary;

		//assert
		Assert.Equal("value", claims["test"]);
	}
}