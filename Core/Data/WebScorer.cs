using System.Collections.Concurrent;
using System.Text.Json;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Data;

public class WebScorer : IDataSource
{
	private readonly IConfig _config;
	public string Name => nameof(WebScorer);

	public string URL(uint courseID)
		=> $"https://api.webscorer.com/racetimer/webscorerapi/results?raceid={courseID}";

	public WebScorer(IConfig config)
		=> _config = config;

	public IReadOnlyCollection<Result> ParseCourse(Course course, JsonElement json)
	{
		var results = json.GetProperty("Racers");

		return results.GetArrayLength() > 0
			? ParseResults(course, results)
			: Array.Empty<Result>();
	}

	private static readonly TimeSpan MinimumDuration = TimeSpan.FromMinutes(4);

	private IReadOnlyCollection<Result> ParseResults(Course course, JsonElement results)
		=> results.EnumerateArray()
			.Where(r => r.GetProperty("Finished").GetByte() == 1
			            && (string.IsNullOrWhiteSpace(r.GetProperty("Distance").GetString())
							|| r.GetProperty("Distance").GetString() == Distance.DefaultKey
			                || r.GetProperty("Distance").GetString() == course.Distance.Display)
			)
			.Select(r => new Result
			{
				Course = course,
				Athlete = ParseAthlete(r),
				StartTime = ParseStart(r.GetProperty("StartTime").GetString()),
				Duration = ParseDuration(r.GetProperty("RaceTime").GetDouble())
			}).Where(r => r.Duration.Value >= MinimumDuration)
			.ToArray();

	private static readonly IDictionary<uint, Athlete> athletes = new ConcurrentDictionary<uint, Athlete>();

	public Athlete ParseAthlete(JsonElement element)
	{
		var id = (_config.Features.GenerateAthleteID
			         ? element.GetProperty("Name").GetString()?.GetID()
			         : null)
		         ?? element.GetProperty("UserId").GetUInt32();

		if (!athletes.ContainsKey(id))
		{
			var hasDOB = DateTime.TryParse(element.GetProperty("Info1").GetString(), out var dob);
			athletes.Add(id, new Athlete
			{
				ID = id,
				Name = element.GetProperty("Name").GetString(),
				Age = element.GetProperty("Age").GetByte(),
				Category = Category.Parse(element.GetProperty("Gender").GetString()),
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