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

public sealed class AthleteSummaryCalculator(IResultService resultService, IConfig config) : IAthleteSummaryCalculator
{
	public async Task<AthleteSummary> GetSummary(Athlete athlete, Iteration iteration)
	{
		var results = await resultService.Find(iteration);
		var iterationCourses = iteration.AllCourses;
		var courses = results.OrderBy(r => iterationCourses.IndexOf(r.Course)).GroupBy(r => r.Course).ToArray();
		var filter = new Filter(Category.Parse(athlete.Category.ToString()));
		var summary = new AthleteSummary
		{
			Athlete = athlete,
			Iteration = iteration,
			AllResults = results,
			Fastest = courses.ToDictionary(c => c.Key, c => c.ToArray().Fastest(filter).Find(r => r.Result.Athlete.Equals(athlete))),
			Average = courses.ToDictionary(c => c.Key, c => c.ToArray().BestAverage(filter).Find(r => r.Result.Athlete.Equals(athlete))),
			Runs = courses.ToDictionary(c => c.Key, c => c.ToArray().MostRuns().Find(r => r.Result.Athlete.Equals(athlete))),
			All = courses.ToDictionary(c => c.Key, c => c.Where(r => r.Athlete.Equals(athlete)).ToArray()),
		};

		if (config.FileSystemResults is not null)
			return summary;

		var overall = new OverallResultsCalculator(iteration);
		return summary with
		{
			Competitions = new[]
				{
					OverallRow("Points/F", Category.F, athlete, () => overall.MostPoints(filter).Find(r => r.Result.Athlete.Equals(athlete))),
					OverallRow("Points/M", Category.M, athlete, () => overall.MostPoints(filter).Find(r => r.Result.Athlete.Equals(athlete))),
					OverallRow("PointsTop3/F", Category.F, athlete, () => overall.MostPoints(3, filter).Find(r => r.Result.Athlete.Equals(athlete))),
					OverallRow("PointsTop3/M", Category.M, athlete, () => overall.MostPoints(3, filter).Find(r => r.Result.Athlete.Equals(athlete))),
					OverallRow("AgeGrade", null, athlete, () => overall.AgeGrade().Find(r => r.Result.Athlete.Equals(athlete))),
					OverallRow("Miles", null, athlete, () => overall.MostMiles().Find(r => r.Result.Athlete.Equals(athlete)))
				}.Where(c => c?.Value != null)
				.ToArray(),

			OverallPoints = overall.MostPoints(filter).Find(r => r.Result.Athlete.Equals(athlete)),
			OverallAgeGrade = overall.AgeGrade().Find(r => r.Result.Athlete.Equals(athlete)),
			OverallMiles = overall.MostMiles().Find(r => r.Result.Athlete.Equals(athlete)),
			TotalResults = summary.Fastest.Count(r => r.Value != null) + summary.Average.Count(r => r.Value != null)
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