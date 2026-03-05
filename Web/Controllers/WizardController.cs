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
public sealed class WizardController(IAthleteService athleteService, IAuthService authService,  IIterationManager iterationManager, ILegacyDataConverter legacyDataConverter, IRegistrationManager registrationManager, IWebScorerAuthenticator webScorer) : Controller
{
	public async Task<IActionResult> Index()
	{
		var athlete = await CurrentAthlete();

		if (athlete is null || athlete.LinkedAccounts.All(a => a.Type != LinkedAccount.Keys.WebScorer))
			return RedirectToAction(nameof(Link));

		return RedirectToAction(nameof(Registration));
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
			return RedirectToAction(nameof(Registration));

		var vm = new ViewModel<string>("Athlete Verification", null);
		return View(vm);
	}

	[HttpPost]
	public async Task<IActionResult> Link(IFormCollection form)
	{
		var athlete = await CurrentAthlete();
		if (athlete is not null && athlete.HasLinkedAccount(LinkedAccount.Keys.WebScorer))
			return RedirectToAction(nameof(Registration));

		try
		{
			var loggedIn = await webScorer.Login(form["Email"], form["Password"]);
			athlete = await legacyDataConverter.GetAthlete(nameof(WebScorer), loggedIn);
			await AddAccountIfNeeded(athlete, LinkedAccount.Keys.Email, form["Email"]);
		}
		catch (HttpRequestException)
		{
			var vm = new ViewModel<string>("Athlete Verification", "Athlete verification failed, please try again");
			return View(vm);
		}

		return RedirectToAction(nameof(Registration));
	}

	private async Task AddAccountIfNeeded(Athlete athlete, string type, string value)
	{
		var account = new LinkedAccount { Type = type, Value = value };
		if (!athlete.LinkedAccounts.Contains(account, LinkedAccount.Comparer))
			await athleteService.AddLinkedAccount(athlete, account);
	}

	[HttpGet]
	public async Task<IActionResult> Registration()
	{
		var athlete = await CurrentAthlete();
		if (!athlete.HasLinkedAccount(LinkedAccount.Keys.WebScorer))
			return RedirectToAction(nameof(Link));

		var iteration = await iterationManager.ActiveIteration();
		if (iteration is null)
			return Redirect("/");

		var vm = new ViewModel<Iteration>("Registration Confirmation", iteration);
		if (!athlete.IsRegistered(iteration))
			return View(vm);

		var user = authService.GetCurrentUser();
		var claims = user.ClaimDictionary;
		await AddAccountIfNeeded(athlete, LinkedAccount.Keys.Discourse, claims["external_id"]);
		await AddAccountIfNeeded(athlete, LinkedAccount.Keys.Email, claims["email"]);

		return View("Complete", vm);
	}

	[HttpPost]
	public async Task<IActionResult> Registration(IFormCollection form)
	{
		var iteration = await iterationManager.ActiveIteration();
		if (iteration is null)
			return Redirect("/");

		await registrationManager.Update(iteration);

		return RedirectToAction(nameof(Registration));
	}
}