using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using Athlete = FLRC.Leaderboards.Model.Athlete;
using AthleteSummary = FLRC.Leaderboards.Web.ViewModels.AthleteSummary;
using SimilarAthlete = FLRC.Leaderboards.Web.ViewModels.SimilarAthlete;

namespace FLRC.Leaderboards.Web.Services;

public sealed class AthleteSummaryCalculator(IResultService resultService, IOverallResultsCalculator overall, ICommunityStarCalculator starCalculator, IConfig config) : IAthleteSummaryCalculator
{
	private Result[] _results;
	private Course[] _iterationCourses;

	public async Task<AthleteSummary> GetSummary(Athlete athlete, Iteration iteration)
	{
		_results ??= await resultService.Find(iteration);
		_iterationCourses ??= iteration.AllCourses;
		var courses = _results.OrderBy(r => _iterationCourses.IndexOf(r.Course)).GroupBy(r => r.Course).ToArray();
		var filter = new Filter(Category.Parse(athlete.Category.ToString()));
		var summary = new AthleteSummary
		{
			Athlete = athlete,
			Iteration = iteration,
			AllResults = _results,
			Fastest = courses.ToDictionary(c => c.Key, c => c.ToArray().Fastest(filter).Find(r => r.Result.Athlete.Equals(athlete))),
			Average = courses.ToDictionary(c => c.Key, c => c.ToArray().BestAverage(filter).Find(r => r.Result.Athlete.Equals(athlete))),
			Runs = courses.ToDictionary(c => c.Key, c => c.ToArray().MostRuns().Find(r => r.Result.Athlete.Equals(athlete))),
			CommunityStars = courses.ToDictionary(c => c.Key, c => c.ToArray().CommunityStars(starCalculator).Find(r => r.Result.Athlete.Equals(athlete))),
			All = courses.ToDictionary(c => c.Key, c => c.Where(r => r.Athlete.Equals(athlete)).ToArray())
		};

		if (config.FileSystemResults is not null)
			return summary;

		var points = overall.MostPoints(iteration, filter).Find(r => r.Result.Athlete.Equals(athlete));
		var pointsTop3 = overall.MostPoints(iteration, 3, filter).Find(r => r.Result.Athlete.Equals(athlete));
		var ageGrade = overall.AgeGrade(iteration).Find(r => r.Result.Athlete.Equals(athlete));
		var miles = overall.MostMiles(iteration).Find(r => r.Result.Athlete.Equals(athlete));
		var mostCourses = overall.MostCourses(iteration).Find(r => r.Result.Athlete.Equals(athlete));
		var stars = overall.Community(iteration).Find(r => r.Result.Athlete.Equals(athlete));
		var team = overall.TeamPoints(iteration).Find(r => r.Value.Team.Equals(athlete.Team(iteration)));
		var total = summary.Fastest.Count(r => r.Value != null) + summary.Average.Count(r => r.Value != null);

		return summary with
		{
			Competitions = new[]
				{
					OverallRow("Points/F", Category.F, athlete, () => points),
					OverallRow("Points/M", Category.M, athlete, () => points),
					OverallRow("PointsTop3/F", Category.F, athlete, () => pointsTop3),
					OverallRow("PointsTop3/M", Category.M, athlete, () => pointsTop3),
					OverallRow("AgeGrade", null, athlete, () => ageGrade),
					OverallRow("Miles", null, athlete, () => miles),
					OverallRow("Courses", null, athlete, () => mostCourses),
					OverallRow("Community", null, athlete, () => stars),
					OverallRow("Team", null, athlete, () => team)
				}.Where(c => c?.Value != null)
				.ToArray(),

			OverallPoints = points,
			OverallAgeGrade = ageGrade,
			OverallMiles = miles,
			OverallCourses = mostCourses,
			OverallCommunityStars = stars,
			TeamResults = team,
			TotalResults = total
		};
	}

	private AthleteOverallRow OverallRow<T>(string id, Category category, Athlete athlete, Func<Ranked<T, Result>> results) where T : Formattable
	{
		if (!config.Competitions.TryGetValue(id, out var name) || category != null && category.Display != athlete.Category.ToString())
			return null;

		var result = results();
		return new AthleteOverallRow
		{
			ID = id,
			Name = name,
			Rank = result?.Rank,
			Value = result?.Value?.Display
		};
	}

	public async Task<SimilarAthlete[]> SimilarAthletes(AthleteSummary summary)
	{
		var courses = summary.AllResults.GroupBy(r => r.Course).ToArray();
		var fastMatches = courses.ToDictionary(c => c.Key, c => c.ToArray().Fastest().Where(r => summary.Fastest.ContainsKey(c.Key) && !r.Result.Athlete.Equals(summary.Athlete) && IsMatch(summary.Fastest[c.Key], r)));
		var avgMatches = courses.ToDictionary(c => c.Key, c => c.ToArray().BestAverage().Where(r => summary.Average.ContainsKey(c.Key) && !r.Result.Athlete.Equals(summary.Athlete) && IsMatch(summary.Average[c.Key], r)));

		var matches = fastMatches.SelectMany(c => c.Value.Select(r => r.Result.Athlete))
			.Union(avgMatches.SelectMany(c => c.Value.Select(r => r.Result.Athlete)))
			.Distinct()
			.ToArray();

		var athletes = new List<AthleteSummary>();
		foreach (var match in matches)
			athletes.Add(await GetSummary(match, summary.Iteration));

		return athletes.Select(their => SimilarAthleteCalculator.GetSimilarity(summary, their))
			.Where(m => Math.Abs(m.FastestPercent.Value) < 10
			            && (m.AveragePercent == null || Math.Abs(m.AveragePercent.Value) < 10)
			            && m.Similarity.Value >= 80)
			.ToArray();
	}

	private const byte PercentThreshold = 5;

	private static bool IsMatch(Ranked<Time, Result> mine, Ranked<Time, Result> theirs)
		=> mine is not null
		   && theirs is not null
		   && Time.AbsolutePercentDifference(mine.Value, theirs.Value) <= PercentThreshold;
}