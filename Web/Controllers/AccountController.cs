using System.Security.Claims;
using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class AccountController(IAthleteService athleteService, IAuthService authService, IDiscourseAuthenticator discourse) : Controller
{
	public RedirectResult Login()
	{
		var host = authService.GetCurrentHost();
		var url = discourse.GetLoginURL(host);
		return Redirect(url);
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

		var athlete = await CurrentAthlete(identity);
		return Redirect(athlete is null ? "/Wizard" : "/");
	}

	private async Task<Athlete> CurrentAthlete(ClaimsIdentity identity)
	{
		var claims = identity.Claims.ToDictionary(c => c.Type, c => c.Value);
		return claims.TryGetValue("external_id", out var id)
			? await athleteService.Find(LinkedAccount.Keys.Discourse, id)
			: null;
	}

	public async Task<RedirectResult> Logout()
	{
		await authService.LogOut();
		return Redirect("/");
	}
}