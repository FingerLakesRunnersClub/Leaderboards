using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class WizardController(IAuthService authService, IAthleteService athleteService, IWebScorerAuthenticator webScorer) : Controller
{
	public async Task<IActionResult> Wizard()
	{
		var athlete = await CurrentAthlete();

		if (athlete is null || athlete.LinkedAccounts.All(a => a.Type != LinkedAccount.Keys.WebScorer))
			return RedirectToAction(nameof(Link));

		return RedirectToAction(nameof(Registration));
	}

	private async Task<Athlete> CurrentAthlete()
	{
		var user = authService.GetCurrentUser();
		var claims = user.Claims.ToDictionary(c => c.Type, c => c.Value);

		return await athleteService.Find("Discourse", claims["external_id"])
		       ?? await athleteService.Find("Email", claims["email"]);
	}

	public async Task<IActionResult> Link()
	{
		var athlete = await CurrentAthlete();
		if (athlete.HasLinkedAccount(LinkedAccount.Keys.WebScorer))
			return RedirectToAction(nameof(Registration));

		var vm = new ViewModel<string>("Athlete Verification", null);
		return View(vm);
	}

	[HttpPost]
	public async Task<IActionResult> Link(IFormCollection form)
	{
		var athlete = await CurrentAthlete();
		if (athlete.HasLinkedAccount(LinkedAccount.Keys.WebScorer))
			return RedirectToAction(nameof(Registration));

		try
		{
			var id = await webScorer.Login(form["Email"], form["Password"]);
			var account = new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = id.ToString() };
			await athleteService.AddLinkedAccount(athlete, account);
		}
		catch (HttpRequestException)
		{
			var vm = new ViewModel<string>("Athlete Verification", "Athlete verification failed, please try again");
			return View(vm);
		}

		return RedirectToAction(nameof(Registration));

	}

	public ViewResult Registration()
	{
		var vm = new ViewModel<string>("Registration Confirmation", null);
		return View(vm);
	}
}