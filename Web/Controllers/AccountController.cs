using System.Security.Claims;
using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class AccountController(IAdminService adminService, IAthleteService athleteService, IAuthService authService, IDiscourseAuthenticator discourse) : Controller
{
	public RedirectResult Login()
	{
		var host = authService.GetCurrentHost();
		var url = discourse.GetLoginURL(host);
		return Redirect(url);
	}

	[HttpGet]
	public async Task<IActionResult> Redirect(string sso, string sig)
	{
		var valid = discourse.IsValidResponse(sso, sig);
		if (!valid)
			return Unauthorized();

		var response = discourse.ParseResponse(sso);
		var identity = new ClaimsIdentity("SSO");
		identity.AddClaims(response.Select(r => new Claim(r.Key, r.Value)));

		var athlete = await CurrentAthlete(identity);
		if (athlete is not null && await adminService.Verify(athlete.ID))
			identity.AddClaim(new Claim(identity.RoleClaimType, nameof(Admin)));

		await authService.LogIn(identity);

		return Redirect(athlete is null ? "/Wizard" : "/");
	}

	private async Task<Athlete> CurrentAthlete(ClaimsIdentity identity)
	{
		var claims = identity.ClaimDictionary;
		return claims.TryGetValue("external_id", out var id)
			? await athleteService.Find(LinkedAccount.Keys.Discourse, id)
			: null;
	}

	[HttpGet]
	public async Task<RedirectResult> Logout()
	{
		await authService.LogOut();
		return Redirect("/");
	}

	public ViewResult AccessDenied()
	{
		var vm = new ViewModel<string>("Access Denied", "You do not have access to this resource.");
		return View(vm);
	}
}