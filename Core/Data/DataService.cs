using System.Collections.Concurrent;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Groups;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FLRC.Leaderboards.Core.Data;

public class DataService : IDataService
{
	private readonly IDataAPI _dataAPI;
	private readonly IGroupAPI _groupAPI;
	private readonly ILogger _logger;
	private readonly TimeSpan _cacheLength;
	private readonly IDictionary<uint, Course> _courses;
	private readonly uint _startListID;

	public DataService(IDataAPI dataAPI, IGroupAPI groupAPI, IConfiguration configuration,
		ILoggerFactory loggerFactory)
	{
		_dataAPI = dataAPI;
		_groupAPI = groupAPI;

		_logger = loggerFactory.CreateLogger("DataService");
		_cacheLength = TimeSpan.FromSeconds(configuration.GetValue<byte?>("APICacheLength") ?? 10);

		_startListID = configuration.GetValue<uint>("StartListRaceID");
		_courses = configuration.GetSection("Courses").GetChildren()
			.ToDictionary(c => uint.Parse(c["ID"]), c => c.Get<Course>());
		foreach (var course in _courses.Values)
			course.Meters = DataParser.ParseDistance(course.Distance);

		CourseNames = _courses.ToDictionary(c => c.Key, c => c.Value.Name);
		Links = configuration.GetSection("Links").GetChildren().ToDictionary(c => c.Key, c => c.Value);
	}

	public IDictionary<uint, string> CourseNames { get; }
	public IDictionary<string, string> Links { get; }

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
				var json = await _dataAPI.GetResults(_startListID);
				foreach (var element in json.GetProperty("Racers").EnumerateArray())
				{
					var athlete = DataParser.ParseAthlete(element);
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
		var course = _courses[id];

		try
		{
			if (course.LastUpdated < DateTime.Now.Subtract(_cacheLength))
			{
				var json = await _dataAPI.GetResults(id);
				var newHash = json.ToString()?.GetHashCode() ?? 0;
				if (newHash != course.LastHash)
				{
					course.Results = DataParser.ParseCourse(course, json);
					course.LastHash = newHash;
				}

				course.LastUpdated = DateTime.Now;
			}
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Could not retrieve results");
		}

		return course;
	}

	public async Task<IEnumerable<Course>> GetAllResults()
	{
		var tasks = _courses.Select(async c => await GetResults(c.Key));
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