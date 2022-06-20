using System.Collections.Concurrent;
using System.Text.Json;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Data;

public class UltraSignup : IDataSource
{
	private readonly IConfig _config;
	public string Name => nameof(UltraSignup);

	public string URL(uint courseID)
		=> $"https://ultrasignup.com/service/events.svc/results/{courseID}/1/json";

	public UltraSignup(IConfig config)
		=> _config = config;

	public IReadOnlyCollection<Result> ParseCourse(Course course, JsonElement json, IDictionary<string, string> aliases)
	{
		return json.EnumerateArray()
			.Where(r => r.GetProperty("status").GetByte() == 1)
			.Select(j => new Result
			{
				Course = course,
				Athlete = ParseAthlete(j, aliases),
				StartTime = new Date(course.Race.Date),
				Duration = ParseDuration(j.GetProperty("time").GetString())
			}).ToArray();
	}

	public static Time ParseDuration(string milliseconds)
		=> new(TimeSpan.FromMilliseconds(double.Parse(milliseconds)));

	private readonly IDictionary<uint, Athlete> athletes = new ConcurrentDictionary<uint, Athlete>();

	public Athlete ParseAthlete(JsonElement element, IDictionary<string, string> aliases)
	{
		var name = element.GetProperty("firstname").GetString() + " " + element.GetProperty("lastname").GetString();
		if (aliases.ContainsKey(name))
		{
			name = aliases[name];
		}

		var id = _config.Features.GenerateAthleteID ? name.GetID() : element.GetProperty("participant_id").GetUInt32();

		return athletes.ContainsKey(id)
			? athletes[id]
			: athletes[id] = new Athlete
			{
				ID = id,
				Name = name,
				Category = Category.Parse(element.GetProperty("gender").GetString()),
				Age = element.GetProperty("age").GetByte()
			};
	}
}