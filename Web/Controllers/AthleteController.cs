using FLRC.AgeGradeCalculator;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Athlete = FLRC.Leaderboards.Model.Athlete;
using AthleteSummary = FLRC.Leaderboards.Web.ViewModels.AthleteSummary;
using Course = FLRC.Leaderboards.Model.Course;
using SimilarAthlete = FLRC.Leaderboards.Web.ViewModels.SimilarAthlete;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class AthleteController(IIterationManager iterationManager, IAthleteService athleteService, ICourseService courseService, IAthleteSummaryCalculator athleteSummaryCalculator, IConfig config) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Index(Guid id)
		=> View(await GetAthlete(id));

	[HttpGet]
	public async Task<ViewResult> Course(Guid id, Guid courseID)
	{
		var athlete = await athleteService.Get(id);
		var course = await courseService.Get(courseID);

		return Enum.TryParse<FieldEvent>(course.Race.Name.ToFieldEvent(), out _)
			? View("FieldEvent", GetFieldEventViewModel(athlete, course))
			: View(GetRunViewModel(athlete, course));
	}

	[HttpGet]
	public async Task<ViewResult> Log(Guid id)
		=> View(await GetLog(id));

	[HttpGet]
	public async Task<ViewResult> Similar(Guid id)
		=> View(await GetSimilarAthletes(id));

	private async Task<ViewModel<AthleteSummary>> GetAthlete(Guid id)
	{
		var iteration = await iterationManager.ActiveIteration();
		var athlete = await athleteService.Get(id);
		var summary = await athleteSummaryCalculator.GetSummary(athlete, iteration);
		return new ViewModel<AthleteSummary>(athlete.Name, summary);
	}

	private static ViewModel<AthleteCourseResults<Time>> GetRunViewModel(Athlete athlete, Course course)
	{
		var myResults = course.Results.Where(r => r.Athlete.Equals(athlete)).ToArray();

		var data = new AthleteCourseResults<Time>
		{
			Athlete = athlete,
			Course = course,
			RankedResults = RankRun(myResults)
		};
		return new ViewModel<AthleteCourseResults<Time>>($"{athlete.Name} — {course.Race.Name}", data);
	}

	private ViewModel<AthleteCourseResults<Performance>> GetFieldEventViewModel(Athlete athlete, Course course)
	{
		var myResults = course.Results.Where(r => r.Athlete.Equals(athlete)).ToArray();

		var data = new AthleteCourseResults<Performance>
		{
			Athlete = athlete,
			Course = course,
			RankedResults = []
		};
		return new ViewModel<AthleteCourseResults<Performance>>($"{athlete.Name} — {course.Race.Name}", data);
	}

	private static RankedList<Time, Result> RankRun(Result[] results)
	{
		var ranks = new RankedList<Time, Result>();

		var sorted = results.OrderBy(r => r.Duration).ToArray();
		for (ushort rank = 1; rank <= sorted.Length; rank++)
		{
			var result = sorted[rank - 1];

			ranks.Add(new Ranked<Time, Result>
			{
				All = ranks,
				Rank = RankRun(result, rank, ranks),
				Result = result,
				Value = new Time(result.Duration),
				AgeGrade = result.AgeGrade()
			});
		}

		return ranks;
	}

	private static Rank RankRun(Result result, ushort rank, RankedList<Time, Result> ranks)
		=> result.Athlete.IsPrivate ? null
			: result.AgeGrade()?.Value > 100 ? new Rank(0)
			: !ranks.Exists(r => r.Rank.Value > 0) ? new Rank(1)
			: ranks.Any() && ranks[^1].Value == new Time(result.Duration) ? ranks.Last().Rank
			: new Rank((ushort)(rank - ranks.Count(r => r.Rank.Value == 0)));


	private static SimilarAthlete[] RankMatches(SimilarAthlete[] matches)
	{
		var ordered = matches.OrderByDescending(r => r.Score).ToArray();
		for (var x = 0; x < ordered.Length; x++)
			ordered[x] = ordered[x] with { Rank = new Rank((ushort)(x + 1)) };
		return ordered;
	}

	private async Task<ViewModel<AthleteLog>> GetLog(Guid id)
	{
		var athlete = await athleteService.Get(id);
		var results = athlete.Results.ToArray();

		var log = new AthleteLog
		{
			Athlete = athlete,
			Results = RankRun(results)
		};
		return new ViewModel<AthleteLog>($"{athlete.Name} — Activity Log", log);
	}

	private async Task<ViewModel<SimilarAthletes>> GetSimilarAthletes(Guid id)
	{
		var iteration = await iterationManager.ActiveIteration();
		var athlete = await athleteService.Get(id);
		var summary = await athleteSummaryCalculator.GetSummary(athlete, iteration);
		var similar = await athleteSummaryCalculator.SimilarAthletes(summary);

		var data = new SimilarAthletes
		{
			Athlete = athlete,
			Matches = RankMatches(similar)
		};
		return new ViewModel<SimilarAthletes>("Similar Athletes", data);
	}
}