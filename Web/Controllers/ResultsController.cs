using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Athlete = FLRC.Leaderboards.Model.Athlete;
using Result = FLRC.Leaderboards.Model.Result;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class ResultsController(IAuthService authService, IAthleteService athleteService, IIterationManager iterationManager, ICourseService courseService, IIterationService iterationService, IResultService resultService, IAdminService adminService) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Fastest(Guid id, [FromQuery] char? c = null, [FromQuery] byte? ag = null, [FromQuery] Guid? i = null)
	{
		var filter = await Filter(c, ag, i);
		var iteration = await iterationManager.ActiveIteration();
		var results = await GetResults(id, iteration, filter, ResultType.Fastest, (r, f) => r.Fastest(f));
		var vm = new ViewModel<CourseResults<Time>>($"{iteration?.Series.Setting[nameof(AppConfig.CourseLabel)]} Results", results);
		return View(vm);
	}

	[HttpGet]
	public async Task<ViewResult> BestAverage(Guid id, char? c = null, byte? ag = null, Guid? i = null)
	{
		var filter = await Filter(c, null, i);
		var iteration = await iterationManager.ActiveIteration();
		var results = await GetResults(id, iteration, filter, ResultType.BestAverage, (r, f) => r.BestAverage(f));
		var vm = new ViewModel<CourseResults<Time>>($"{iteration?.Series.Setting[nameof(AppConfig.CourseLabel)]} Results", results);
		return View(vm);
	}

	[HttpGet]
	public async Task<ViewResult> MostRuns(Guid id, char? c = null, byte? ag = null, Guid? i = null)
	{
		var filter = await Filter(null, null, i);
		var iteration = await iterationManager.ActiveIteration();
		var results = await GetResults(id, iteration, filter, ResultType.MostRuns, (r, f) => r.MostRuns(f));
		var vm = new ViewModel<CourseResults<ushort>>($"{iteration?.Series.Setting[nameof(AppConfig.CourseLabel)]} Results", results);
		return View(vm);
	}

	private async Task<Filter> Filter(char? c, byte? ag, Guid? i)
		=> new()
		{
			Category = c is not null ? Category.Parse(c.ToString()) : null,
			AgeGroup = ag is not null ? Team.Teams[ag.Value] : null,
			Iteration = await FilterIteration(i)
		};

	private async Task<Iteration> FilterIteration(Guid? i)
	{
		if (i is null)
			return await iterationManager.ActiveIteration();

		return i == Guid.Empty
			? null
			: await iterationService.Get(i.Value);
	}

	private async Task<CourseResults<T>> GetResults<T>(Guid id, Iteration iteration, Filter filter, ResultType type, Func<Result[], Filter, RankedList<T, Result>> resultsFunc)
	{
		var course = await courseService.Get(id);
		var results = await resultService.Find(id);
		return new CourseResults<T>
		{
			ActiveIteration = iteration,
			Course = course,
			Type = type,
			Filter = filter,
			Results = resultsFunc(results, filter)
		};
	}

	[HttpGet]
	[Authorize]
	public async Task<ViewResult> Add(Guid id)
	{
		var course = await courseService.Get(id);
		var result = new Result
		{
			Course = course,
			Athlete = await CurrentAthlete(),
			StartTime = DateTime.Now
		};
		var vm = new ViewModel<Result>("Add Result", result);
		return View("Form", vm);
	}

	[HttpPost]
	[Authorize]
	public async Task<IActionResult> Add(Guid id, IFormCollection form)
	{
		if (!byte.TryParse(form["Duration[h]"], out var hours)
		    || !byte.TryParse(form["Duration[m]"], out var minutes)
		    || !byte.TryParse(form["Duration[s]"], out var seconds))
			throw new ArgumentException(nameof(Result.Duration));

		var result = new Result
		{
			Course = await courseService.Get(id),
			Athlete = await CurrentAthlete(),
			StartTime = DateTime.Parse(form["StartTime"]),
			Duration = new TimeSpan(hours, minutes, seconds)
		};

		if (!result.IsValid())
		{
			var vm = new ViewModel<Result>("Add Result", result)
			{
				Error = "The result entered is invalid, please double-check your entry!"
			};
			return View("Form", vm);
		}

		await resultService.Add(result);
		return RedirectToAction(nameof(Fastest), new { id });
	}

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> Edit(Guid id)
	{
		var athlete = await CurrentAthlete();
		var result = await resultService.Get(id);

		if (result.Athlete != athlete && !await adminService.Verify(athlete.ID))
			return Forbid();

		var vm = new ViewModel<Result>("Edit Result", result);
		return View("Form", vm);
	}

	[HttpPost]
	[Authorize]
	public async Task<IActionResult> Edit(Guid id, IFormCollection form)
	{
		var athlete = await CurrentAthlete();
		var result = await resultService.Get(id);

		if (result.Athlete != athlete && !await adminService.Verify(athlete.ID))
			return Forbid();

		if (!byte.TryParse(form["Duration[h]"], out var hours)
		    || !byte.TryParse(form["Duration[m]"], out var minutes)
		    || !byte.TryParse(form["Duration[s]"], out var seconds))
			throw new ArgumentException(nameof(Result.Duration));

		var updated = new Result
		{
			Course = result.Course,
			Athlete = result.Athlete,
			StartTime = DateTime.Parse(form["StartTime"]),
			Duration = new TimeSpan(hours, minutes, seconds)
		};

		if (!updated.IsValid())
		{
			var vm = new ViewModel<Result>("Add Result", result)
			{
				Error = "The result entered is invalid, please double-check your entry!"
			};
			return View("Form", vm);
		}

		await resultService.Update(result, updated);
		return RedirectToAction(nameof(AthleteController.Log), nameof(Athlete), new { id = result.AthleteID });
	}

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> Delete(Guid id)
	{
		var athlete = await CurrentAthlete();
		var result = await resultService.Get(id);

		if (result.Athlete != athlete && !await adminService.Verify(athlete.ID))
			return Forbid();

		var vm = new ViewModel<Result>("Delete Result", result);
		return View(vm);
	}

	[HttpPost]
	[Authorize]
	public async Task<IActionResult> Delete(Guid id, IFormCollection form)
	{
		var athlete = await CurrentAthlete();
		var result = await resultService.Get(id);

		if (result.Athlete != athlete && !await adminService.Verify(athlete.ID))
			return Forbid();

		await resultService.Delete(result);
		return RedirectToAction(nameof(AthleteController.Log), nameof(Athlete), new { id = result.AthleteID });
	}

	private async Task<Athlete> CurrentAthlete()
	{
		var user = authService.GetCurrentUser();
		var claims = user.ClaimDictionary;

		return await athleteService.Find(LinkedAccount.Keys.Discourse, claims["external_id"]);
	}
}