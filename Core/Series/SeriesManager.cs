using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Series;

public sealed class SeriesManager : ISeriesManager
{
	private readonly IDataService _dataService;
	private readonly SeriesSet _series;

	public SeriesManager(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_series = config.Series;
	}

	public async Task<IDictionary<Series, RankedList<SeriesResult>>> Earliest()
		=> await RankedResult(Earliest);

	public async Task<IDictionary<Series, RankedList<SeriesResult>>> Fastest()
		=> await RankedResult(Fastest);

	private async Task<IDictionary<Series, RankedList<SeriesResult>>> RankedResult(Func<SeriesResult[], RankedList<SeriesResult>> method)
	{
		var results = await GetSeriesResults();
		return results.ToDictionary(s => s.Key, s => method(s.Value));
	}

	private async Task<IDictionary<Series, SeriesResult[]>> GetSeriesResults()
	{
		var courses = await _dataService.GetAllResults();

		return courses
			.SelectMany(c => c.Results)
			.GroupBy(r => r.Athlete)
			.SelectMany(a => FindCompletions(a.ToArray()))
			.GroupBy(r => r.Series)
			.ToDictionary(s => s.Key, s => s.ToArray());
	}

	private SeriesResult[] FindCompletions(Result[] athletesResults)
	{
		var completions = new List<SeriesResult>();
		var sorted = athletesResults
			.OrderBy(r => r.StartTime)
			.ToArray();

		foreach (var result in sorted)
		{
			if (result.StartTime.Value < completions.LastOrDefault()?.FinishTime.Value)
				continue;

			var completion = FindCompletion(sorted, result);
			if (completion == null)
				continue;

			completions.Add(completion);
		}

		return completions.ToArray();
	}

	private SeriesResult FindCompletion(Result[] athletesResults, Result first)
		=> _series.Select(s => FindCompletion(s, athletesResults, first))
			.FirstOrDefault(completion => completion != null);

	private static SeriesResult FindCompletion(Series series, Result[] athletesResults, Result first)
	{
		var endLimit = first.StartTime.Value.AddHours(series.HourLimit);
		var matches = athletesResults
			.Where(r => r.StartTime.Value >= first.StartTime.Value && r.FinishTime.Value <= endLimit)
			.ToArray();

		if (matches.Length < series.Races.Length)
			return null;

		var results = new List<Result>();
		foreach (var race in series.Races)
		{
			var match = matches.FirstOrDefault(r => r.CourseID == race);
			if (match == null)
				return null;

			results.Add(match);
		}

		var last = results.MaxBy(r => r.FinishTime);
		return new SeriesResult
		{
			Series = series,
			Athlete = first.Athlete,
			StartTime = first.StartTime,
			FinishTime = last.FinishTime,
			RunningTime = new Time(TimeSpan.FromMilliseconds(results.Sum(r => r.Duration.Value.TotalMilliseconds))),
			TotalTime = new Time(last.FinishTime.Value - first.StartTime.Value),
			Results = results.ToArray()
		};
	}

	private static RankedList<SeriesResult> Earliest(SeriesResult[] results)
		=> Rank(results, r => r.FinishTime);

	private static RankedList<SeriesResult> Fastest(SeriesResult[] results)
		=> Rank(results, r => r.TotalTime);

	private static RankedList<SeriesResult> Rank<T>(SeriesResult[] results, Func<SeriesResult, T> sort)
	{
		var ranks = new RankedList<SeriesResult>();

		var sorted = results.OrderBy(sort).ToArray();
		for (ushort rank = 1; rank <= sorted.Length; rank++)
		{
			var result = sorted[rank - 1];
			var seriesResult = new Ranked<SeriesResult>
			{
				All = ranks,
				Rank = new Rank(rank),
				Value = result,
				AgeGrade = !result.Athlete.Private
					? new AgeGrade(result.Results.Average(r => r.AgeGrade ?? 0))
					: null
			};
			ranks.Add(seriesResult);
		}

		return ranks;
	}
}