using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Data;

public static class ResultFileReader
{
	public static Result ParseResult(Course course, string line, IDictionary<string, string> aliases)
	{
		var name = line[2..34].Trim();
		var age = line[52..54].Trim();
		var category = line[38..39].Trim();
		var performance = line[60..].Trim();
		var formatted = performance.Split(":").Length switch
		{
			1 => "0:00:0" + performance,
			2 => "0:0" + performance,
			_ => performance
		};
		var isTime = TimeSpan.TryParse(formatted, out var time);

		if (aliases.TryGetValue(name, out var alias))
			name = alias;

		return new Result
		{
			Course = course,
			Athlete = new Athlete
			{
				ID = name.GetID(),
				Name = name,
				Age = byte.Parse(age),
				Category = Category.Parse(category)
			},
			StartTime = new Date(course.Race.Date),
			Duration = isTime ? course.FormatTime(time) : null,
			Performance = !isTime ? new Performance(performance) : null
		};
	}
}