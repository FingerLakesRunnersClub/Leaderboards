using System.Collections.Concurrent;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Groups;
using FLRC.Leaderboards.Core.Races;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FLRC.Leaderboards.Core.Data;

public class DataService : IDataService
{
	private readonly IDictionary<string, IDataAPI> _dataAPI;
	private readonly IGroupAPI _groupAPI;
	private readonly ILogger _logger;
	private readonly TimeSpan _cacheLength;
	private readonly IDictionary<uint, Race> _races;
	private readonly uint _startListID;

	public DataService(IDictionary<string, IDataAPI> dataAPI, IGroupAPI groupAPI, IConfiguration configuration,
		ILoggerFactory loggerFactory)
	{
		_dataAPI = dataAPI;
		_groupAPI = groupAPI;

		_logger = loggerFactory.CreateLogger("DataService");
		_cacheLength = TimeSpan.FromSeconds(configuration.GetValue<byte?>("APICacheLength") ?? 10);

		_startListID = configuration.GetValue<uint>("StartListRaceID");
		_races = configuration.GetSection("Races").GetChildren()
			.ToDictionary(c => uint.Parse(c["ID"]), GetRace);
	}

	private static Race GetRace(IConfigurationSection section)
	{
		var race = section.Get<Race>();
		race.Courses = GetCourses(race, section);
		return race;
	}

	private static IReadOnlyList<Course> GetCourses(Race race, IConfigurationSection section)
		=> section.GetSection("Courses").GetChildren()
			.Select(c => new Course
			{
				Race = race,
				ID = uint.Parse(c.Value),
				Distance = new Distance(c.Key == "Default" ? section["Distance"] : c.Key)
			}).ToList();

	private readonly IDictionary<uint, Athlete> _athletes = new ConcurrentDictionary<uint, Athlete>();
	private DateTime _athleteCacheTimestamp;

	public async Task<Athlete> GetAthlete(uint id)
	{
		try
		{
			var athletes = await GetAthletes();
			return athletes[id];
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Could not retrieve athletes");
			return null;
		}
	}

	public async Task<IDictionary<uint, Athlete>> GetAthletes()
	{
		if (_startListID == 0)
		{
			var results = await GetAllResults();
			return results.SelectMany(c => c.Results.Select(r => r.Athlete))
				.DistinctBy(a => a.ID)
				.ToDictionary(a => a.ID, a => a);
		}

		try
		{
			if (_athleteCacheTimestamp < DateTime.Now.Subtract(_cacheLength))
			{
				var json = await _dataAPI[nameof(WebScorer)].GetResults(_startListID);
				foreach (var athlete in json.GetProperty("Racers").EnumerateArray().Select(WebScorer.ParseAthlete))
				{
					_athletes[athlete.ID] = athlete;
				}

				_athleteCacheTimestamp = DateTime.Now;
			}
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Could not retrieve athletes");
		}

		return _athletes;
	}

	public async Task<Course> GetResults(uint id)
	{
		var courses = _races.SelectMany(r => r.Value.Courses).Where(c => c.ID == id).ToList();
		var updates = courses.Select(UpdateCourse);
		await Task.WhenAll(updates);
		return courses.First();
	}

	private async Task UpdateCourse(Course course)
	{
		try
		{
			if (course.LastUpdated < DateTime.Now.Subtract(_cacheLength))
			{
				var json = await _dataAPI[course.Race.Source].GetResults(course.ID);
				var newHash = json.ToString().GetHashCode();
				if (newHash != course.LastHash)
				{
					course.Results = _dataAPI[course.Race.Source].Source.ParseCourse(course, json);
					course.LastHash = newHash;
				}

				course.LastUpdated = DateTime.Now;
			}
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Could not retrieve results");
		}
	}

	public async Task<IEnumerable<Course>> GetAllResults()
	{
		var tasks = _races.Select(async c => await GetResults(c.Key));
		return await Task.WhenAll(tasks);
	}

	public async Task<IEnumerable<Athlete>> GetGroupMembers(string id)
	{
		try
		{
			var groups = await GetGroups();
			return groups[id];
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, $"Could not retrieve group members for {id}");
			return new List<Athlete>();
		}
	}


	private IDictionary<string, IEnumerable<Athlete>> _groups;
	private DateTime _groupCacheTimestamp;

	private async Task<IDictionary<string, IEnumerable<Athlete>>> GetGroups()
	{
		if (_groupCacheTimestamp < DateTime.Now.Subtract(_cacheLength))
		{
			var athletes = await GetAthletes();
			var members = await _groupAPI.GetGroups();
			_groups = members.ToDictionary(m => m.Key,
				m => m.Value.Select(v => athletes.ContainsKey(v) ? athletes[v] : null).Where(a => a != null));
			_groupCacheTimestamp = DateTime.Now;
		}

		return _groups;
	}
}