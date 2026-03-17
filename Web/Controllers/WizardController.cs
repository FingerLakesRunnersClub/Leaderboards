using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

[Authorize]
public sealed class WizardController(IAthleteService athleteService, IAuthService authService, ILegacyDataConverter legacyDataConverter, IWebScorerAuthenticator webScorer) : Controller
{
	public async Task<IActionResult> Index()
	{
		var athlete = await CurrentAthlete();

		if (athlete is null || athlete.LinkedAccounts.All(a => a.Type != LinkedAccount.Keys.WebScorer))
			return RedirectToAction(nameof(Link));

		return RedirectToAction(nameof(Complete));
	}

	private async Task<Athlete> CurrentAthlete()
	{
		var user = authService.GetCurrentUser();
		var claims = user.ClaimDictionary;

		return await athleteService.Find("Discourse", claims["external_id"])
		       ?? await athleteService.Find("Email", claims["email"]);
	}

	[HttpGet]
	public async Task<IActionResult> Link()
	{
		var athlete = await CurrentAthlete();
		if (athlete is not null && athlete.HasLinkedAccount(LinkedAccount.Keys.WebScorer))
			return RedirectToAction(nameof(Complete));

		var vm = new ViewModel<string>("Athlete Verification", null);
		return View(vm);
	}

	[HttpPost]
	public async Task<IActionResult> Link(IFormCollection form)
	{
		var athlete = await CurrentAthlete();
		if (athlete is not null && athlete.HasLinkedAccount(LinkedAccount.Keys.WebScorer))
			return RedirectToAction(nameof(Complete));

		try
		{
			var loggedIn = await webScorer.Login(form["Email"], form["Password"]);
			athlete = await legacyDataConverter.GetAthlete(nameof(WebScorer), loggedIn);

			var user = authService.GetCurrentUser();
			var claims = user.ClaimDictionary;

			await AddAccountIfNeeded(athlete, LinkedAccount.Keys.Discourse, claims["external_id"]);
			await AddAccountIfNeeded(athlete, LinkedAccount.Keys.Email, claims["email"]);
			await AddAccountIfNeeded(athlete, LinkedAccount.Keys.Email, form["Email"]);
		}
		catch (HttpRequestException)
		{
			var vm = new ViewModel<string>("Athlete Verification", "Athlete verification failed, please try again");
			return View(vm);
		}

		return RedirectToAction(nameof(Complete));
	}

	[HttpGet]
	public async Task<ViewResult> Complete()
	{
		var athlete = await CurrentAthlete();
		var vm = new ViewModel<Athlete>("Verification Complete", athlete);
		return View(vm);
	}

	private async Task AddAccountIfNeeded(Athlete athlete, string type, string value)
	{
		var account = new LinkedAccount { Type = type, Value = value };
		if (!athlete.LinkedAccounts.Contains(account, LinkedAccount.Comparer))
			await athleteService.AddLinkedAccount(athlete, account);
	}
}