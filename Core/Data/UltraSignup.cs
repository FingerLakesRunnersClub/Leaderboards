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

	public IReadOnlyCollection<Result> ParseCourse(Course course, JsonElement json)
	{
		return json.EnumerateArray()
			.Where(r => r.GetProperty("status").GetByte() == 1)
			.Select(j => new Result
			{
				Course = course,
				Athlete = ParseAthlete(j),
				StartTime = new Date(course.Race.Date),
				Duration = ParseDuration(j.GetProperty("time").GetString())
			}).ToArray();
	}

	private static Time ParseDuration(string milliseconds)
		=> new(TimeSpan.FromMilliseconds(double.Parse(milliseconds)));

	private static readonly IDictionary<uint, Athlete> athletes = new ConcurrentDictionary<uint, Athlete>();

	public Athlete ParseAthlete(JsonElement element)
	{
		var name = element.GetProperty("firstname").GetString() + " " + element.GetProperty("lastname").GetString();
		var id = _config.Features.GenerateAthleteID ? name.GetID() : element.GetProperty("participant_id").GetUInt32();

		if (!athletes.ContainsKey(id))
		{
			athletes.Add(id, new Athlete
			{
				ID = id,
				Name = name,
				Category = DataParser.ParseCategory(element.GetProperty("gender").GetString()),
				Age = element.GetProperty("age").GetByte()
			});
		}

		return athletes[id];
	}
}