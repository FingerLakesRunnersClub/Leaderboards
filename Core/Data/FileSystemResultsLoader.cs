using System.IO.Abstractions;
using System.Text.RegularExpressions;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Data;

public sealed class FileSystemResultsLoader : IFileSystemResultsLoader
{
	private static readonly Regex RegexPattern = Patterns.TrackFile();
	private readonly IFileSystem _fs;
	private readonly IConfig _config;

	public FileSystemResultsLoader(IFileSystem fs, IConfig config)
	{
		_fs = fs;
		_config = config;
	}

	private Race[] _cachedRaces;

	public Race[] GetRaces()
	{
		_cachedRaces ??= _fs.Directory
			.GetFiles(_config.FileSystemResults, "*.txt", SearchOption.AllDirectories)
			.Select(p => RegexPattern.Match(p))
			.Select(GetRace)
			.OrderBy(r => r.Courses[0].Distance?.Meters ?? Distance.MetersPerMarathon)
			.ToArray();

		_config.CourseNames = _cachedRaces
			.DistinctBy(r => r.ID)
			.ToDictionary(r => r.ID, r => r.Name);

		return _cachedRaces.GroupBy(r => r.ID).Select(r => r.MaxBy(r2 => r2.Courses.Sum(c => c.Results.Length))).ToArray();
	}

	private static Race GetRace(Match info)
	{
		var name = info.Groups[2].Value;
		var date = info.Groups[1].Value;

		var race = new Race
		{
			ID = name.GetID(),
			Name = name,
			Date = DateTime.Parse(date),
			Type = "Track",
			Source = "File"
		};

		var distance = new Distance(race.Name);
		var course = new Course
		{
			ID = race.ID,
			Race = race,
			Distance = distance.Meters > 0 ? distance : null,
			ShowDecimals = ShouldShowDecimals(distance)
		};

		race.Courses = [course];
		return race;
	}

	private static bool ShouldShowDecimals(Distance distance)
		=> distance.Meters is > 0 and <= 5000;

	private Course[] _allResults;

	public async Task<Course[]> GetAllResults()
	{
		if (_allResults is not null)
			return _allResults;

		var paths = _cachedRaces.Select(r => Path.Combine(_config.FileSystemResults, r.Date.ToString("yyyy-MM-dd"), $"{r.Name}.txt"));
		var tasks = paths
			.Select(async path => await _fs.File.ReadAllTextAsync(path))
			.Select(async task => GetCourse(await task));
		var courses = await Task.WhenAll(tasks);
		var events = courses.GroupBy(c => c.Name);
		return _allResults = events.Select(g => new Course { ID = g.First().ID, Race = g.First().Race, ShowDecimals = g.First().ShowDecimals, Results = g.SelectMany(r => r.Results).OrderBy(r => r.Duration).ToArray() }).ToArray();
	}

	private Course GetCourse(string file)
	{
		var lines = file.Split(Environment.NewLine);
		var date = DateTime.Parse(lines[0].Trim());
		var name = lines[1].Trim();

		var race = _cachedRaces.First(r => r.Date == date && r.Name == name);
		var distance = new Distance(name);

		var course = new Course
		{
			ID = race.ID,
			Race = race,
			Distance = distance,
			ShowDecimals = ShouldShowDecimals(distance)
		};

		course.Results = lines.Skip(5).SkipLast(1).Select(l => ParseResult(course, l)).ToArray();
		return course;
	}

	private static Result ParseResult(Course course, string l)
	{
		var name = l[2..34].Trim();
		var age = l[52..54].Trim();
		var category = l[38..39].Trim();
		var performance = l[62..].Trim();
		var formatted = performance.Split(":").Length switch
		{
			1 => "0:00:0" + performance,
			2 => "0:0" + performance,
			_ => performance
		};
		var isTime = TimeSpan.TryParse(formatted, out var time);

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
			Performance = !isTime ? new Distance(performance) : null
		};
	}
}