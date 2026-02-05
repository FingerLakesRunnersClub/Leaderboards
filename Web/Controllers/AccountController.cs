using System.Security.Claims;
using System.Text.Json;
using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class AccountController(IAuthService authService, IDiscourseAuthenticator auth, IHttpContextAccessor contextAccessor) : ControllerBase
{
	public ContentResult Info()
	{
		var user = authService.GetCurrentUser();
		var fields = user.Claims.ToDictionary(c => c.Type, c => c.Value);
		return new ContentResult
		{
			Content = JsonSerializer.Serialize(fields),
			ContentType = "application/json"
		};
	}


	public RedirectResult Login()
	{
		var request = contextAccessor.HttpContext!.Request;
		var currentHost = $"{request.Scheme}://{request.Host}";
		var url = auth.GetLoginURL(currentHost);
		return base.Redirect(url);
	}

	public async Task<IActionResult> Redirect(string sso, string sig)
	{
		var valid = auth.IsValidResponse(sso, sig);
		if (!valid)
			return Unauthorized();

		var response = auth.ParseResponse(sso);
		var identity = new ClaimsIdentity("SSO");
		identity.AddClaims(response.Select(r => new Claim(r.Key, r.Value)));
		await authService.LogIn(identity);
		return RedirectToAction(nameof(Info));
	}

	public async Task<RedirectResult> Logout()
	{
		await authService.LogOut();
		return Redirect("/");
	}
}