using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.ViewModels;

namespace FLRC.Leaderboards.Web.Services;

public static class UltraChallengeResultsCalculator
{
	public static async Task<IDictionary<Challenge, RankedList<ChallengeResult, Result>>> Earliest(Iteration iteration)
		=> await RankedResult(iteration, Earliest);

	public static async Task<IDictionary<Challenge, RankedList<ChallengeResult, Result>>> Fastest(Iteration iteration)
		=> await RankedResult(iteration, Fastest);

	private static async Task<IDictionary<Challenge, RankedList<ChallengeResult, Result>>> RankedResult(Iteration iteration, Func<ChallengeResult[], RankedList<ChallengeResult, Result>> method)
	{
		var results = await GetChallengeResults(iteration);
		return results.ToDictionary(s => s.Key, s => method(s.Value));
	}

	private static async Task<IDictionary<Challenge, ChallengeResult[]>> GetChallengeResults(Iteration iteration)
	{
		var iterationStart = iteration.StartDate?.ToDateTime(TimeOnly.MinValue);
		var iterationEnd = iteration.EndDate?.ToDateTime(TimeOnly.MaxValue);
		var courses = iteration.UltraChallenges.SelectMany(c => c.Courses).Distinct().ToArray();
		return courses
			.SelectMany(c => c.Results.Where(r => r.StartTime >= iterationStart && r.FinishTime <= iterationEnd))
			.GroupBy(r => r.Athlete)
			.SelectMany(a => FindCompletions(iteration.UltraChallenges, a.ToArray()))
			.GroupBy(r => r.Challenge)
			.ToDictionary(s => s.Key, s => s.ToArray());
	}

	private static ChallengeResult[] FindCompletions(Challenge[] challenges, Result[] athletesResults)
	{
		var completions = new List<ChallengeResult>();
		var sorted = athletesResults
			.OrderBy(r => r.StartTime)
			.ToArray();

		foreach (var result in sorted)
		{
			if (result.StartTime < completions.LastOrDefault()?.FinishTime.Value)
				continue;

			var completion = FindCompletion(challenges, sorted, result);
			if (completion == null)
				continue;

			completions.Add(completion);
		}

		return completions.ToArray();
	}

	private static ChallengeResult FindCompletion(Challenge[] challenges, Result[] athletesResults, Result first)
		=> challenges.Select(challenge => FindCompletion(challenge, athletesResults, first))
			.FirstOrDefault(completion => completion is not null);

	private static ChallengeResult FindCompletion(Challenge challenge, Result[] athletesResults, Result first)
	{
		var endLimit = challenge.TimeLimit.HasValue ? first.StartTime.Add(challenge.TimeLimit.Value) : (DateTime?)null;
		var matches = athletesResults
			.Where(r => challenge.Courses.Any(c => c.ID == r.CourseID) && r.StartTime >= first.StartTime && (endLimit is null || r.FinishTime <= endLimit))
			.ToArray();

		if (matches.Length < challenge.Courses.Count)
			return null;

		var results = challenge.Courses
			.Select(c => matches.FirstOrDefault(r => r.CourseID == c.ID))
			.Where(r => r is not null)
			.ToArray();

		if (results.Length < challenge.Courses.Count)
			return null;

		var last = results.MaxBy(r => r.FinishTime);
		return new ChallengeResult
		{
			Challenge = challenge,
			Athlete = first.Athlete,
			StartTime = new Date(first.StartTime),
			FinishTime = new Date(last.FinishTime),
			RunningTime = new Time(TimeSpan.FromMilliseconds(results.Sum(r => r.Duration.TotalMilliseconds))),
			TotalTime = new Time(last.FinishTime - first.StartTime),
			Results = results.ToArray()
		};
	}

	private static RankedList<ChallengeResult, Result> Earliest(ChallengeResult[] results)
		=> Rank(results, r => r.FinishTime);

	private static RankedList<ChallengeResult, Result> Fastest(ChallengeResult[] results)
		=> Rank(results, r => r.TotalTime);

	private static RankedList<ChallengeResult, Result> Rank<T>(ChallengeResult[] results, Func<ChallengeResult, T> sort)
	{
		var ranks = new RankedList<ChallengeResult, Result>();

		var sorted = results.OrderBy(sort).ToArray();
		for (ushort rank = 1; rank <= sorted.Length; rank++)
		{
			var challengeResult = sorted[rank - 1];
			var rankedResult = new Ranked<ChallengeResult, Result>
			{
				All = ranks,
				Rank = new Rank(rank),
				Value = challengeResult,
				AgeGrade = !challengeResult.Athlete.IsPrivate
					? new AgeGrade(challengeResult.Results.Average(r => r.AgeGrade()?.Value ?? 0))
					: null
			};
			ranks.Add(rankedResult);
		}

		return ranks;
	}
}