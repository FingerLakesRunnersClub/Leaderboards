using System.Text.Json;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Data;

public class UltraSignup : IDataSource
{
	public string Name => nameof(UltraSignup);

	public string URL(uint courseID)
		=> $"https://ultrasignup.com/service/events.svc/results/{courseID}/1/json";

	public IEnumerable<Result> ParseCourse(Course course, JsonElement json)
	{
		return json.EnumerateArray()
			.Where(r => r.GetProperty("status").GetByte() == 1)
			.Select(j => new Result
			{
				Course = course,
				Athlete = ParseAthlete(j),
				Duration = ParseDuration(j.GetProperty("time").GetString())
			});
	}

	private static Time ParseDuration(string milliseconds)
		=> new(TimeSpan.FromMilliseconds(double.Parse(milliseconds)));

	private static Athlete ParseAthlete(JsonElement element)
	{
		var name = element.GetProperty("firstname").GetString() + " " + element.GetProperty("lastname").GetString();
		return new Athlete
		{
			ID = (uint) name.GetHashCode(),
			Name = name,
			Category = DataParser.ParseCategory(element.GetProperty("gender").GetString()),
			Age = element.GetProperty("age").GetByte()
		};
	}
}