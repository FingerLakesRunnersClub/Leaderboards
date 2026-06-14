using FLRC.Leaderboards.Core.Data;
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

		var challenge = athlete.Challenges.FirstOrDefault(c => c.Iteration == iteration && c.IsPrimary);
		if (challenge is null)
			return Content(string.Empty);

		var allIterationResults = athlete.Results.For(iteration);

		var challengeCoursesCompleted = challenge.Courses
			.Where(c => allIterationResults.Any(r => r.CourseID == c.ID))
			.ToArray();

		var percentComplete = challenge.Courses.Count > 0
			? 100 * challengeCoursesCompleted.Length / challenge.Courses.Count
			: 0;

		var progress = new ChallengeProgress
		{
			Athlete = athlete,
			Challenge = challenge,
			CompletedCourses = challengeCoursesCompleted,
			AllCourses = challenge.Courses.ToArray(),
			PercentComplete = percentComplete,
			ShowLinkButton = false
		};
		return View("../AthleteProgress", progress);
	}
}