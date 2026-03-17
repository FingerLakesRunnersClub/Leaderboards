using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

[Authorize]
public sealed class ChallengeController(IAuthService authService, IAthleteService athleteService, IChallengeService challengeService, IIterationManager iterationManager, IRegistrationManager registrationManager) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Dashboard()
	{
		var athlete = await CurrentAthlete();

		var iteration = await iterationManager.ActiveIteration();
		if (iteration is null)
			return Redirect("/");

		if (!athlete.IsRegistered(iteration))
			return RedirectToAction(nameof(Registration));

		var dashboard = new ChallengeDashboard();
		var vm = new ViewModel<ChallengeDashboard>("Challenge Dashboard", dashboard);
		return View(vm);
	}

	private async Task<Athlete> CurrentAthlete()
	{
		var user = authService.GetCurrentUser();
		var claims = user.ClaimDictionary;

		return await athleteService.Find("Discourse", claims["external_id"]);
	}

	[HttpGet]
	public async Task<IActionResult> Registration()
	{
		var athlete = await CurrentAthlete();

		var iteration = await iterationManager.ActiveIteration();
		if (iteration is null)
			return Redirect("/");

		var vm = new ViewModel<Iteration>("Registration Confirmation", iteration);
		if (athlete.IsRegistered(iteration))
			return RedirectToAction(nameof(Select));

		return View(vm);
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

	[HttpGet]
	public async Task<IActionResult> Select()
	{
		var iteration = await iterationManager.ActiveIteration();
		if (iteration?.OfficialChallenge is null)
			throw new NotImplementedException("No official challenge has been created yet");

		var athlete = await CurrentAthlete();
		if (!athlete.IsRegistered(iteration))
			return RedirectToAction(nameof(Registration));

		if (athlete.HasChallenge(iteration))
			return RedirectToAction(nameof(Dashboard));

		var form = new SelectChallengeForm
		{
			Iteration = iteration,
			Official = iteration.OfficialChallenge,
			Courses = iteration.Races.SelectMany(r => r.Courses).ToArray()
		};

		var vm = new ViewModel<SelectChallengeForm>("Challenge Selection", form);
		return View(vm);
	}

	[HttpPost]
	public async Task<IActionResult> Select(SelectChallengeForm form)
	{
		var iteration = await iterationManager.ActiveIteration();
		if (iteration?.OfficialChallenge is null)
			return Redirect("/");

		var athlete = await CurrentAthlete();
		if (!athlete.IsRegistered(iteration))
			return RedirectToAction(nameof(Registration));

		if (athlete.HasChallenge(iteration))
			return RedirectToAction(nameof(Dashboard));

		var courses = form.Selection == "Official"
			? iteration.OfficialChallenge.Courses
			: SelectedCourses(iteration, form.Selected);

		var confirmation = new SelectChallengeForm
		{
			Iteration = iteration,
			Selection = form.Selection,
			Selected = form.Selected,
			Courses = courses.ToArray()
		};

		var vm = new ViewModel<SelectChallengeForm>("Confirm Challenge Selection", confirmation);
		return View("Confirm", vm);
	}

	private static IEnumerable<Course> SelectedCourses(Iteration iteration, Guid[] ids)
		=> iteration.Races.SelectMany(r => r.Courses).Where(c => ids.Contains(c.ID));

	[HttpPost]
	public async Task<IActionResult> Confirm(SelectChallengeForm form)
	{
		var iteration = await iterationManager.ActiveIteration();
		if (iteration is null)
			return Redirect("/");

		var athlete = await CurrentAthlete();
		if (!athlete.IsRegistered(iteration))
			return RedirectToAction(nameof(Registration));

		if (athlete.HasChallenge(iteration))
			return RedirectToAction(nameof(Dashboard));

		if (form.Selection == "Official")
		{
			var official = iteration.Challenges.First(c => c is { IsPrimary: true, IsOfficial: true });
			await challengeService.AddConnection(athlete, official);
			return RedirectToAction(nameof(Dashboard));
		}

		var challenge = new Challenge
		{
			Iteration = iteration,
			Athlete = athlete,
			Name = $"{athlete.Name} Personal Challenge",
			Courses = SelectedCourses(iteration, form.Selected).ToArray()
		};
		await challengeService.Add(challenge);
		await challengeService.AddConnection(athlete, challenge);

		return RedirectToAction(nameof(Dashboard));
	}
}