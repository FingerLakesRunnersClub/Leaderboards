using System.Collections.Concurrent;
using System.Text.Json;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Data;

public sealed class UltraSignup : IDataSource
{
	private readonly IConfig _config;
	public string Name => nameof(UltraSignup);

	public string URL(uint courseID)
		=> $"https://ultrasignup.com/service/events.svc/results/{courseID}/1/json";

	public UltraSignup(IConfig config)
		=> _config = config;

	private static readonly byte[] ValidStatusTypes = [1, 6];
	public Result[] ParseCourse(Course course, JsonElement json, IDictionary<string, string> aliases)
		=> json.EnumerateArray()
			.Where(r => ValidStatusTypes.Contains(r.GetProperty("status").GetByte()))
			.Select(j => new Result
			{
				Course = course,
				Athlete = ParseAthlete(j, aliases),
				StartTime = new Date(course.Race.Date),
				Duration = ParseDuration(j.GetProperty("time").GetString())
			}).ToArray();

	public static Time ParseDuration(string milliseconds)
		=> new(TimeSpan.FromMilliseconds(double.Parse(milliseconds)));

	private readonly ConcurrentDictionary<uint, Athlete> athletes = new();

	public Athlete ParseAthlete(JsonElement element, IDictionary<string, string> aliases)
	{
		var name = element.GetProperty("firstname").GetString() + " " + element.GetProperty("lastname").GetString();
		if (aliases.TryGetValue(name, out var alias))
		{
			name = alias;
		}

		var id = _config.Features.GenerateAthleteID ? name.GetID() : element.GetProperty("participant_id").GetUInt32();

		return athletes.TryGetValue(id, out var athlete)
			? athlete
			: athletes[id] = new Athlete
			{
				ID = id,
				Name = name,
				Category = Category.Parse(element.GetProperty("gender").GetString()),
				Age = element.GetProperty("age").GetByte()
			};
	}
}