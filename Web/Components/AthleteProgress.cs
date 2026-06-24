using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Components;

public sealed class AthleteProgress(IIterationManager iterationManager) : ViewComponent
{
	public async Task<IViewComponentResult> InvokeAsync(Athlete athlete)
	{
		var iteration = await iterationManager.ActiveIteration();
		if (iteration is null
		    || iteration.EndDate < DateOnly.FromDateTime(DateTime.Today)
		    || iteration.RegistrationType == nameof(WebScorer) && !athlete.IsRegistered(iteration))
			return Content(string.Empty);

		var progress = GetProgress(athlete, iteration, false);
		if (progress is null)
			return Content(string.Empty);

		return View("../AthleteProgress", progress);
	}

	public static ChallengeProgress GetProgress(Athlete athlete, Iteration iteration, bool showLinkButton)
	{
		var challenge = athlete.Challenges.FirstOrDefault(c => c.Iteration == iteration && c.IsPrimary);
		if (challenge is null)
			return null;

		var results = athlete.Results.For(iteration);
		var courses = challenge.Courses.ToArray();
		var completed = courses
			.Where(c => results.Any(r => r.CourseID == c.ID))
			.ToArray();

		var coursePercent = courses.Length > 0
			? 100 * completed.Length / courses.Length
			: 0;

		var completedMiles = completed.Sum(c => new Distance(c.DistanceDisplay).Miles);
		var totalMiles = courses.Sum(c => new Distance(c.DistanceDisplay).Miles);

		var mileagePercent = totalMiles > 0
			? 100 * completedMiles / totalMiles
			: 0;

		return new ChallengeProgress
		{
			Athlete = athlete,
			Challenge = challenge,
			CompletedCourses = completed,
			AllCourses = courses,
			CompletedMiles = completedMiles,
			TotalMiles = totalMiles,
			CoursePercent = coursePercent,
			MileagePercent = mileagePercent,
			ShowLinkButton = showLinkButton
		};
	}
}