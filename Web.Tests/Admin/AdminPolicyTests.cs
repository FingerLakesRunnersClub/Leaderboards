using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class AdminPolicyTests
{
	[Fact]
	public async Task SucceedsIfUserIsAdmin()
	{
		//arrange
		var identity = new ClaimsIdentity();
		identity.AddClaim(new Claim(identity.RoleClaimType, nameof(Admin)));

		var policy = new Areas.Admin.Policies.Admin();
		var context = new AuthorizationHandlerContext([policy], new ClaimsPrincipal(identity), null);

		//act
		await policy.HandleAsync(context);

		//assert
		Assert.True(context.HasSucceeded);
		Assert.False(context.HasFailed);
	}

	[Fact]
	public async Task FailsIfUserIsNotAdmin()
	{
		//arrange
		var identity = new ClaimsIdentity();
		var policy = new Areas.Admin.Policies.Admin();
		var context = new AuthorizationHandlerContext([policy], new ClaimsPrincipal(identity), null);

		//act
		await policy.HandleAsync(context);

		//assert
		Assert.False(context.HasSucceeded);
		Assert.True(context.HasFailed);
	}
}