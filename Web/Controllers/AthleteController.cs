using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public class AthleteController : Controller
{
	private readonly IDataService _dataService;
	private readonly IConfig _config;

	public AthleteController(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_config = config;
	}

	public async Task<ViewResult> Index(uint id) => View(await GetAthlete(id));

	public async Task<ViewResult> Course(uint id, uint courseID, string distance) => View(await GetResults(id, courseID, distance));

	public async Task<ViewResult> Log(uint id) => View(await GetLog(id));

	public async Task<ViewResult> Similar(uint id) => View(await GetSimilarAthletes(id));

	private async Task<AthleteSummaryViewModel> GetAthlete(uint id)
	{
		var athlete = await _dataService.GetAthlete(id);
		var courses = (await _dataService.GetAllResults()).ToArray();
		var summary = new AthleteSummary(athlete, courses, _config);

		return new AthleteSummaryViewModel
		{
			Config = _config,
			Summary = summary
		};
	}

	private async Task<AthleteCourseResultsViewModel> GetResults(uint id, uint courseID, string distance)
	{
		var course = await _dataService.GetResults(courseID, distance);
		var results = course.Results.Where(r => r.Athlete.ID == id).ToArray();

		return new AthleteCourseResultsViewModel
		{
			Config = _config,
			Athlete = results.First().Athlete,
			Course = course,
			RankedResults = Rank(results)
		};
	}

	private static RankedList<Time> Rank(IReadOnlyCollection<Result> results)
	{
		var ranks = new RankedList<Time>();

		var sorted = results.OrderBy(r => r.Duration.Value).ToArray();
		for (ushort rank = 1; rank <= sorted.Length; rank++)
		{
			var result = sorted[rank - 1];

			ranks.Add(new Ranked<Time>
			{
				All = ranks,
				Rank = result.AgeGrade >= 100 ? new Rank(0)
					: !ranks.Any(r => r.Rank.Value > 0) ? new Rank(1)
					: ranks.Any() && ranks.Last().Value == result.Duration ? ranks.Last().Rank
					: new Rank((ushort)(rank - ranks.Count(r => r.Rank.Value == 0))),
				Result = result,
				Value = result.Duration,
				AgeGrade = new AgeGrade(result.AgeGrade)
			});
		}

		return ranks;
	}

	private static IReadOnlyCollection<SimilarAthlete> Rank(IReadOnlyCollection<SimilarAthlete> matches)
	{
		var ordered = matches.OrderByDescending(r => r.Score).ToArray();
		for (var x = 0; x < ordered.Length; x++)
			ordered[x].Rank = new Rank((ushort)(x + 1));
		return ordered;
	}

	private async Task<AthleteLogViewModel> GetLog(uint id)
	{
		var athlete = await _dataService.GetAthlete(id);
		var courses = await _dataService.GetAllResults();
		var results = courses.SelectMany(c => c.Results.Where(r => r.Athlete.ID == athlete.ID)).ToArray();

		return new AthleteLogViewModel
		{
			Config = _config,
			Athlete = athlete,
			Results = Rank(results)
		};
	}

	private async Task<SimilarAthletesViewModel> GetSimilarAthletes(uint id)
	{
		var athlete = await _dataService.GetAthlete(id);
		var results = (await _dataService.GetAllResults()).ToArray();
		var my = new AthleteSummary(athlete, results, _config);

		return new SimilarAthletesViewModel
		{
			Config = _config,
			Athlete = my.Athlete,
			Matches = Rank(my.SimilarAthletes)
		};
	}
}