using System.Security.Claims;
using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class AccountController(IAuthService authService, IDiscourseAuthenticator discourse) : Controller
{
	public RedirectResult Login()
	{
		var host = authService.GetCurrentHost();
		var url = discourse.GetLoginURL(host);
		return base.Redirect(url);
	}

	public async Task<IActionResult> Redirect(string sso, string sig)
	{
		var valid = discourse.IsValidResponse(sso, sig);
		if (!valid)
			return Unauthorized();

		var response = discourse.ParseResponse(sso);
		var identity = new ClaimsIdentity("SSO");
		identity.AddClaims(response.Select(r => new Claim(r.Key, r.Value)));
		await authService.LogIn(identity);

		return Redirect("/Wizard");
	}

	public async Task<RedirectResult> Logout()
	{
		await authService.LogOut();
		return Redirect("/");
	}
}