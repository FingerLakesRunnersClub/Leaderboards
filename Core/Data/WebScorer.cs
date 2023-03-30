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

	public IReadOnlyCollection<Result> ParseCourse(Course course, JsonElement json, IDictionary<string, string> aliases)
	{
		var results = json.GetProperty("Racers");

		return results.GetArrayLength() > 0
			? ParseResults(course, results, aliases)
			: Array.Empty<Result>();
	}

	private static readonly TimeSpan MinimumDuration = TimeSpan.FromMinutes(4);

	private IReadOnlyCollection<Result> ParseResults(Course course, JsonElement results, IDictionary<string, string> aliases)
		=> results.EnumerateArray()
			.Where(r => r.GetProperty("Finished").GetByte() == 1
			            && (string.IsNullOrWhiteSpace(r.GetProperty("Distance").GetString())
			                || r.GetProperty("Distance").GetString() == Distance.DefaultKey
			                || r.GetProperty("Distance").GetString() == course.Distance.Display)
			)
			.Select(r => GetResult(course, r, ParseAthlete(r, aliases)))
			.Where(r => r.Duration is null || r.Duration.Value >= MinimumDuration)
			.ToArray();

	private static Result GetResult(Course course, JsonElement r, Athlete athlete)
	{
		return new Result
		{
			Course = course,
			Athlete = athlete,
			StartTime = ParseStart(r.GetProperty("StartTime").GetString()),
			Duration = !athlete.Private
				? ParseDuration(r.GetProperty("RaceTime").GetDouble())
				: null
		};
	}

	private readonly IDictionary<uint, Athlete> athletes = new ConcurrentDictionary<uint, Athlete>();

	public Athlete ParseAthlete(JsonElement element, IDictionary<string, string> aliases)
	{
		var name = element.GetProperty("Name").GetString() ?? "(unknown)";
		if (aliases.ContainsKey(name))
		{
			name = aliases[name];
		}

		var id = (_config.Features.GenerateAthleteID
			         ? name?.GetID()
			         : null)
		         ?? element.GetProperty("UserId").GetUInt32();

		if (athletes.ContainsKey(id))
		{
			return athletes[id];
		}

		return athletes[id] = new Athlete
		{
			ID = id,
			Name = name,
			Age = element.GetProperty("Age").GetByte(),
			Category = Category.Parse(element.GetProperty("Gender").GetString()),
			DateOfBirth = GetDOB(element),
			Private = IsPrivate(element)
		};
	}

	private DateTime? GetDOB(JsonElement element)
		=> _config.BirthdateField is not null
		   && DateTime.TryParse(element.GetProperty(_config.BirthdateField).GetString(), out var dob)
			? dob
			: null;

	private bool IsPrivate(JsonElement element)
		=> _config.PrivateField is not null
		   && element.GetProperty(_config.PrivateField).GetString()?.ToLowerInvariant() == "yes";

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