using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using FLRC.Leaderboards.Core.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class AccountController(IDiscourseAuthenticator auth, IHttpContextAccessor contextAccessor) : ControllerBase
{
	public ContentResult Info()
	{
		var fields = contextAccessor.HttpContext!.User.Claims.ToDictionary(c => c.Type, c => c.Value);
		return new ContentResult
		{
			Content = JsonSerializer.Serialize(fields),
			ContentType = "application/json"
		};
	}

	public RedirectResult Login()
	{
		var currentHost = $"{Request.Scheme}://{Request.Host}";
		var url = auth.GetLoginURL(currentHost);
		return base.Redirect(url);
	}

	public async Task<IActionResult> Redirect(string sso, string sig)
	{
		var valid = auth.IsValidResponse(sso, sig);
		if (!valid)
			return Unauthorized();

		var response = auth.ParseResponse(sso);
		var identity = new GenericIdentity(response["username"]);
		identity.AddClaims(response.Select(r => new Claim(r.Key, r.Value)));
		await contextAccessor.HttpContext!.SignInAsync(new ClaimsPrincipal(identity));
		return RedirectToAction(nameof(Info));
	}

	public async Task<RedirectResult> Logout()
	{
		await contextAccessor.HttpContext!.SignOutAsync();
		return Redirect("/");
	}
}