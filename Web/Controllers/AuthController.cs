using System.Security.Claims;
using System.Security.Principal;
using FLRC.Leaderboards.Core.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class AuthController : ControllerBase
{
	private readonly IDiscourseAuthenticator _auth;
	private readonly HttpContext _context;

	public AuthController(IDiscourseAuthenticator auth, IHttpContextAccessor contextAccessor)
	{
		_auth = auth;
		_context = contextAccessor.HttpContext!;
	}

	public IActionResult Info()
		=> new ContentResult
		{
			Content = "{" + string.Join(",", _context.User.Claims.Select(c => $@"""{c.Type}"":""{c.Value}""")) + "}",
			ContentType = "application/json"
		};

	public IActionResult Login()
	{
		var currentHost = $"{Request.Scheme}://{Request.Host}";
		var url = _auth.GetLoginURL(currentHost);
		return base.Redirect(url);
	}

	public async Task<IActionResult> Redirect(string sso, string sig)
	{
		var valid = _auth.IsValidResponse(sso, sig);
		if (!valid)
			return Unauthorized();

		var response = _auth.ParseResponse(sso);
		var identity = new GenericIdentity(response["username"]);
		identity.AddClaims(response.Select(r => new Claim(r.Key, r.Value)));
		await _context.SignInAsync(new ClaimsPrincipal(identity));
		return RedirectToAction(nameof(Info));
	}

	public async Task<IActionResult> Logout()
	{
		await _context.SignOutAsync();
		return Redirect("/");
	}
}