using System.Collections.Concurrent;
using System.Text.Json;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Data;

public sealed class WebScorer : IDataSource
{
	private readonly IConfig _config;
	public string Name => nameof(WebScorer);

	public const string DefaultDistance = "Default";

	public string URL(uint courseID)
		=> $"https://api.webscorer.com/racetimer/webscorerapi/results?raceid={courseID}";

	public WebScorer(IConfig config)
		=> _config = config;

	public Result[] ParseCourse(Course course, JsonElement json, IDictionary<string, string> aliases)
	{
		var results = json.GetProperty("Racers");

		return results.GetArrayLength() > 0
			? ParseResults(course, results, aliases)
			: [];
	}

	private static readonly TimeSpan MinimumDuration = TimeSpan.FromMinutes(4);

	private Result[] ParseResults(Course course, JsonElement results, IDictionary<string, string> aliases)
		=> results.EnumerateArray()
			.Where(r => r.GetProperty("Finished").GetByte() == 1
	            && (_config.Features.GenerateAthleteID
					|| r.GetProperty("UserId").GetUInt32() > 0)
	            && (string.IsNullOrWhiteSpace(r.GetProperty("Distance").GetString())
	               || r.GetProperty("Distance").GetString() == DefaultDistance
	               || r.GetProperty("Distance").GetString() == course?.ShortName)
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

	private readonly ConcurrentDictionary<uint, Athlete> _athletes = new();

	public Athlete ParseAthlete(JsonElement element, IDictionary<string, string> aliases)
	{
		var name = element.GetProperty(nameof(Name)).GetString() ?? "(unknown)";
		if (name.Contains(','))
		{
			var split = name.Split(',', 2);
			name = $"{split[1].Trim()} {split[0].Trim()}";
		}
		if (aliases is not null && aliases.TryGetValue(name, out var alias))
		{
			name = alias;
		}

		var id = (_config.Features.GenerateAthleteID
			         ? name?.GetID()
			         : null)
		         ?? element.GetProperty("UserId").GetUInt32();

		var nowPrivate = IsPrivate(element);
		if (_athletes.TryGetValue(id, out var athlete))
		{
			athlete.Private = _athletes[id].Private || nowPrivate;
			return athlete;
		}

		return _athletes[id] = new Athlete
		{
			ID = id,
			Name = name,
			Age = element.GetProperty("Age").GetByte(),
			Category = Category.Parse(element.GetProperty("Gender").GetString()),
			DateOfBirth = GetDOB(element),
			Email = element.GetProperty("EmailAddress").GetString(),
			Private = nowPrivate
		};
	}

	private static DateTime? GetDOB(JsonElement element)
		=> element.TryGetProperty("Info1", out var prop) && DateTime.TryParse(prop.GetString(), out var dob)
			? dob
			: null;

	private static bool IsPrivate(JsonElement element)
		=> element.TryGetProperty("Info2", out var prop) && prop.GetString() == "Y";

	private static Date ParseStart(string value)
		=> DateTime.TryParse(value, out var start)
			? new Date(start)
			: null;

	public static Time ParseDuration(double seconds)
		=> new(TimeSpan.FromSeconds(Math.Ceiling(Math.Round(seconds, 1, MidpointRounding.ToZero))));
}