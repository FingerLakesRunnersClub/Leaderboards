using System.Security.Claims;
using System.Security.Principal;
using FLRC.Leaderboards.Core.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace FLRC.Leaderboards.Web.Services;

public sealed class AuthService(IDiscourseAuthenticator discourse, IHttpContextAccessor contextAccessor) : IAuthService
{
	public async Task LogIn(IIdentity identity)
		=> await contextAccessor.HttpContext!.SignInAsync(new ClaimsPrincipal(identity));

	public bool IsLoggedIn()
		=> GetCurrentUser()?.Identity?.IsAuthenticated ?? false;

	public ClaimsPrincipal GetCurrentUser()
		=> contextAccessor.HttpContext!.User;

	public async Task LogOut()
		=> await contextAccessor.HttpContext!.SignOutAsync();
}