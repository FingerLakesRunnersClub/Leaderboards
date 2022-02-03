using System.Collections.Concurrent;
using System.Text.Json;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Data;

public class WebScorer : IDataSource
{
	public string Name => nameof(WebScorer);

	public string URL(uint courseID)
		=> $"https://api.webscorer.com/racetimer/webscorerapi/results?raceid={courseID}";

	public IEnumerable<Result> ParseCourse(Course course, JsonElement json)
	{
		var results = json.GetProperty("Racers");

		return results.GetArrayLength() > 0
			? ParseResults(course, results)
			: Array.Empty<Result>();
	}

	private static readonly TimeSpan MinimumDuration = TimeSpan.FromMinutes(4);

	private static IEnumerable<Result> ParseResults(Course course, JsonElement results)
		=> results.EnumerateArray()
			.Where(r => r.GetProperty("Finished").GetByte() == 1)
			.Select(r => new Result
			{
				Course = course,
				Athlete = ParseAthlete(r),
				StartTime = ParseStart(r.GetProperty("StartTime").GetString()),
				Duration = ParseDuration(r.GetProperty("RaceTime").GetDouble())
			}).Where(r => r.Duration.Value >= MinimumDuration);

	private static readonly IDictionary<uint, Athlete> athletes = new ConcurrentDictionary<uint, Athlete>();

	public static Athlete ParseAthlete(JsonElement result)
	{
		var id = result.GetProperty("UserId").GetUInt32();
		if (id == 0)
		{
			id = (uint?)result.GetProperty("Name").GetString()?.GetHashCode(StringComparison.InvariantCultureIgnoreCase) ?? 0;
		}

		if (!athletes.ContainsKey(id))
		{
			var hasDOB = DateTime.TryParse(result.GetProperty("Info1").GetString(), out var dob);
			athletes.Add(id, new Athlete
			{
				ID = id,
				Name = result.GetProperty("Name").GetString(),
				Age = result.GetProperty("Age").GetByte(),
				Category = DataParser.ParseCategory(result.GetProperty("Gender").GetString()),
				DateOfBirth = hasDOB ? dob : null
			});
		}

		return athletes[id];
	}

	private static Date ParseStart(string value)
	{
		var isDate = DateTime.TryParse(value, out var start);
		return isDate
			? new Date(start)
			: null;
	}

	public static Time ParseDuration(double seconds)
		=> new(TimeSpan.FromSeconds(Math.Ceiling(Math.Round(seconds, 1, MidpointRounding.ToZero))));
}