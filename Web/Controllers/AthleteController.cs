using System.Collections.ObjectModel;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Course = FLRC.Leaderboards.Core.Races.Course;
using Result = FLRC.Leaderboards.Core.Results.Result;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class AthleteController(IIterationManager iterationManager, IDataService dataService, IConfig config) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Index(uint id)
		=> View(await GetAthlete(id));

	[HttpGet]
	public async Task<ViewResult> Course(uint id, uint courseID, string name)
	{
		var allResults = await dataService.GetAllResults();
		var badges = GetBadges(id, allResults);
		var course = await dataService.GetResults(courseID, name);

		return course.IsFieldEvent
			? View("FieldEvent", GetFieldEventViewModel(id, course, await badges))
			: View(GetRunViewModel(id, course, await badges));
	}

	[HttpGet]
	public async Task<ViewResult> Log(uint id)
		=> View(await GetLog(id));

	[HttpGet]
	public async Task<ViewResult> Similar(uint id)
		=> View(await GetSimilarAthletes(id));

	private async Task<AthleteSummaryViewModel> GetAthlete(uint id)
	{
		var athlete = await dataService.GetAthlete(id);
		var results = await dataService.GetAllResults();
		var summary = new AthleteSummary(athlete, results, config);

		return new AthleteSummaryViewModel
		{
			Config = config,
			Summary = summary,
			Header = new AthleteHeader
			{
				Athlete = athlete,
				BadgeIcons = await GetBadges(id, results),
				ShowLink = false
			}
		};
	}

	private AthleteCourseResultsViewModel GetRunViewModel(uint id, Course course, IDictionary<string, string> badges)
	{
		var myResults = course.Results.Where(r => r.Athlete.ID == id).ToArray();

		return new AthleteCourseResultsViewModel
		{
			Config = config,
			Header = new AthleteHeader
			{
				Athlete = myResults[0].Athlete,
				BadgeIcons = badges,
				ShowLink = true
			},
			Course = course,
			RankedResults = RankRun(myResults)
		};
	}

	private AthleteFieldEventResultsViewModel GetFieldEventViewModel(uint id, Course course, IDictionary<string, string> badges)
	{
		var myResults = course.Results.Where(r => r.Athlete.ID == id).ToArray();

		return new AthleteFieldEventResultsViewModel()
		{
			Config = config,
			Header = new AthleteHeader
			{
				Athlete = myResults[0].Athlete,
				BadgeIcons = badges,
				ShowLink = true
			},
			Course = course,
			RankedResults = RankFieldEvent(myResults)
		};
	}

	private static RankedList<Time, Result> RankRun(Result[] results)
	{
		var ranks = new RankedList<Time, Result>();

		var sorted = results.OrderBy(r => r.Duration ?? Time.Max).ToArray();
		for (ushort rank = 1; rank <= sorted.Length; rank++)
		{
			var result = sorted[rank - 1];

			ranks.Add(new Ranked<Time>
			{
				All = ranks,
				Rank = RankRun(result, rank, ranks),
				Result = result,
				Value = result.Duration,
				AgeGrade = result.AgeGrade()
			});
		}

		return ranks;
	}

	private static RankedList<Performance, Result> RankFieldEvent(Result[] results)
	{
		var ranks = new RankedList<Performance, Result>();

		var sorted = results.OrderBy(r => r.Performance ?? Performance.Zero).ToArray();
		for (ushort rank = 1; rank <= sorted.Length; rank++)
		{
			var result = sorted[rank - 1];

			ranks.Add(new Ranked<Performance, Result>
			{
				All = ranks,
				Rank = RankFieldEvent(result, rank, ranks),
				Result = result,
				Value = result.Performance,
				AgeGrade = result.AgeGrade()
			});
		}

		return ranks;
	}

	private static Rank RankRun(Result result, ushort rank, RankedList<Time, Result> ranks)
		=> result.Athlete.Private ? null
			: result.AgeGrade > 100 ? new Rank(0)
			: !ranks.Exists(r => r.Rank.Value > 0) ? new Rank(1)
			: ranks.Any() && ranks[^1].Value == result.Duration ? ranks.Last().Rank
			: new Rank((ushort)(rank - ranks.Count(r => r.Rank.Value == 0)));


	private static Rank RankFieldEvent(Result result, ushort rank, RankedList<Performance, Result> ranks)
		=> result.Athlete.Private ? null
			: result.AgeGrade > 100 ? new Rank(0)
			: !ranks.Exists(r => r.Rank.Value > 0) ? new Rank(1)
			: ranks.Any() && ranks[^1].Value == result.Performance ? ranks.Last().Rank
			: new Rank((ushort)(rank - ranks.Count(r => r.Rank.Value == 0)));

	private static SimilarAthlete[] RankMatches(SimilarAthlete[] matches)
	{
		var ordered = matches.OrderByDescending(r => r.Score).ToArray();
		for (var x = 0; x < ordered.Length; x++)
			ordered[x].Rank = new Rank((ushort)(x + 1));
		return ordered;
	}

	private async Task<IDictionary<string, string>> GetBadges(uint id, Course[] results)
	{
		var iteration = await iterationManager.ActiveIteration();
		if (iteration is null)
			return ReadOnlyDictionary<string, string>.Empty;

		var myID = Guid.NewGuid();

		var overall = new OverallResults(results);
		var completed = overall.Completed().Any(r => r.Result.Athlete.ID == id);

		var challengeResults = await UltraChallengeResultsCalculator.Earliest(iteration);
		var result = challengeResults
			.Where(s => s.Value.Any(r => r.Value.Athlete.ID == myID))
			.ToDictionary(s => BadgeIcon(s.Key), s => s.Key.Name);

		return completed
			? result.Prepend(new KeyValuePair<string, string>("medal", config.App)).ToDictionary(d => d.Key, d => d.Value)
			: result;
	}

	private static string BadgeIcon(Challenge challenge)
		=> challenge switch
		{
			{ IsOfficial: true, IsPrimary: true } => "medal",
			{ IsOfficial: true, IsPrimary: false } when challenge.Name.Contains("Tarmac") => "road",
			{ IsOfficial: true, IsPrimary: false } when challenge.Name.Contains("Trail") => "mountain",
			{ IsOfficial: true, IsPrimary: false } when challenge.Name.Contains("Ultra") => "diamond",
			_ => throw new ArgumentOutOfRangeException($"Invalid challenge {challenge.Name}")
		};

	private async Task<AthleteLogViewModel> GetLog(uint id)
	{
		var athlete = await dataService.GetAthlete(id);
		var courses = await dataService.GetAllResults();
		var results = courses.SelectMany(c => c.Results.Where(r => r.Athlete.ID == athlete.ID)).ToArray();

		return new AthleteLogViewModel
		{
			Config = config,
			Header = new AthleteHeader
			{
				Athlete = athlete,
				BadgeIcons = await GetBadges(id, courses),
				ShowLink = true
			},
			Results = RankRun(results)
		};
	}

	private async Task<SimilarAthletesViewModel> GetSimilarAthletes(uint id)
	{
		var athlete = await dataService.GetAthlete(id);
		var results = await dataService.GetAllResults();
		var my = new AthleteSummary(athlete, results, config);

		return new SimilarAthletesViewModel
		{
			Config = config,
			Header = new AthleteHeader
			{
				Athlete = my.Athlete,
				BadgeIcons = await GetBadges(id, results),
				ShowLink = true
			},
			Matches = RankMatches(my.SimilarAthletes())
		};
	}
}