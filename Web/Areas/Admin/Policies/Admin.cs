using Microsoft.AspNetCore.Authorization;

namespace FLRC.Leaderboards.Web.Areas.Admin.Policies;

public sealed class Admin : AuthorizationHandler<Admin>, IAuthorizationRequirement
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, Admin requirement)
	{
		if (context.User.IsInRole(nameof(Admin)))
			context.Succeed(requirement);
		else
			context.Fail();

		return Task.CompletedTask;
	}
}