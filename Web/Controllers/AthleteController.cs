using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Series;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class AthleteController : Controller
{
	private readonly IDataService _dataService;
	private readonly ISeriesManager _seriesManager;
	private readonly IConfig _config;

	public AthleteController(IDataService dataService, ISeriesManager seriesManager, IConfig config)
	{
		_dataService = dataService;
		_seriesManager = seriesManager;
		_config = config;
	}

	public async Task<ViewResult> Index(uint id) => View(await GetAthlete(id));

	public async Task<ViewResult> Course(uint id, uint courseID, string name) => View(await GetResults(id, courseID, name));

	public async Task<ViewResult> Log(uint id) => View(await GetLog(id));

	public async Task<ViewResult> Similar(uint id) => View(await GetSimilarAthletes(id));

	private async Task<AthleteSummaryViewModel> GetAthlete(uint id)
	{
		var athlete = await _dataService.GetAthlete(id);
		var results = await _dataService.GetAllResults();
		var summary = new AthleteSummary(athlete, results, _config);

		return new AthleteSummaryViewModel
		{
			Config = _config,
			Summary = summary,
			Header = new AthleteHeader
			{
				Athlete = athlete,
				BadgeIcons = await GetBadges(id, results),
				ShowLink = false
			}
		};
	}

	private async Task<AthleteCourseResultsViewModel> GetResults(uint id, uint courseID, string name)
	{
		var results = await _dataService.GetAllResults();
		var course = await _dataService.GetResults(courseID, name);
		var myResults = course.Results.Where(r => r.Athlete.ID == id).ToArray();

		return new AthleteCourseResultsViewModel
		{
			Config = _config,
			Header = new AthleteHeader
			{
				Athlete = myResults[0].Athlete,
				BadgeIcons = await GetBadges(id, results),
				ShowLink = true
			},
			Course = course,
			RankedResults = Rank(myResults)
		};
	}

	private static RankedList<Time> Rank(Result[] results)
	{
		var ranks = new RankedList<Time>();

		var sorted = results.OrderBy(r => r.Duration ?? Time.Max).ToArray();
		for (ushort rank = 1; rank <= sorted.Length; rank++)
		{
			var result = sorted[rank - 1];

			ranks.Add(new Ranked<Time>
			{
				All = ranks,
				Rank = Rank(result, rank, ranks),
				Result = result,
				Value = result.Duration,
				AgeGrade = AgeGrade(result)
			});
		}

		return ranks;
	}

	private static Rank Rank(Result result, ushort rank, RankedList<Time> ranks)
		=> result.Athlete.Private ? null
			: result.AgeGrade >= 100 ? new Rank(0)
			: !ranks.Exists(r => r.Rank.Value > 0) ? new Rank(1)
			: ranks.Any() && ranks[^1].Value == result.Duration ? ranks.Last().Rank
			: new Rank((ushort)(rank - ranks.Count(r => r.Rank.Value == 0)));

	private static AgeGrade AgeGrade(Result result)
		=> result.AgeGrade is not null
			? new AgeGrade(result.AgeGrade.Value)
			: null;

	private static SimilarAthlete[] Rank(SimilarAthlete[] matches)
	{
		var ordered = matches.OrderByDescending(r => r.Score).ToArray();
		for (var x = 0; x < ordered.Length; x++)
			ordered[x].Rank = new Rank((ushort)(x + 1));
		return ordered;
	}

	private async Task<IDictionary<string, string>> GetBadges(uint id, Course[] results)
	{
		var overall = new OverallResults(results);
		var completed = overall.Completed().Any(r => r.Result.Athlete.ID == id);

		var series = await _seriesManager.Earliest();
		var result = series
			.Where(s => s.Value.Any(r => r.Value.Athlete.ID == id))
			.ToDictionary(s => s.Key.BadgeIcon, s => s.Key.Name);

		return completed
			? result.Prepend(new KeyValuePair<string, string>("medal", _config.App)).ToDictionary(d => d.Key, d => d.Value)
			: result;
	}

	private async Task<AthleteLogViewModel> GetLog(uint id)
	{
		var athlete = await _dataService.GetAthlete(id);
		var courses = await _dataService.GetAllResults();
		var results = courses.SelectMany(c => c.Results.Where(r => r.Athlete.ID == athlete.ID)).ToArray();

		return new AthleteLogViewModel
		{
			Config = _config,
			Header = new AthleteHeader
			{
				Athlete = athlete,
				BadgeIcons = await GetBadges(id, courses),
				ShowLink = true
			},
			Results = Rank(results)
		};
	}

	private async Task<SimilarAthletesViewModel> GetSimilarAthletes(uint id)
	{
		var athlete = await _dataService.GetAthlete(id);
		var results = await _dataService.GetAllResults();
		var my = new AthleteSummary(athlete, results, _config);

		return new SimilarAthletesViewModel
		{
			Config = _config,
			Header = new AthleteHeader
			{
				Athlete = my.Athlete,
				BadgeIcons = await GetBadges(id, results),
				ShowLink = true
			},
			Matches = Rank(my.SimilarAthletes())
		};
	}
}