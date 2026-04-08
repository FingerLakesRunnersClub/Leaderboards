using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Areas.Admin.Controllers;

[Area(nameof(Admin))]
[Authorize(nameof(Admin))]
public sealed class ChallengesController(IChallengeService challengeService, ICourseService courseService) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Index()
	{
		var challenges = await challengeService.All();

		var vm = new ViewModel<Challenge[]>("Challenges", challenges);
		return View(vm);
	}

	[HttpGet]
	public async Task<ViewResult> Edit(Guid id)
	{
		var challenge = await challengeService.Get(id);
		var vm = new ViewModel<Challenge>("Edit Challenge", challenge);
		return View("Form", vm);
	}

	[HttpPost]
	public async Task<RedirectToActionResult> Edit(Guid id, string name, byte? timeLimit, Guid[] selected)
	{
		var challenge = await challengeService.Get(id);
		var updated = new Challenge
		{
			Name = name,
			TimeLimit = timeLimit is not null ? TimeSpan.FromHours(timeLimit.Value) : null
		};
		var courses = await courseService.GetCourses(selected);
		await challengeService.Update(challenge, updated);
		await challengeService.UpdateCourses(challenge, courses);
		return RedirectToAction(nameof(Index));
	}
}