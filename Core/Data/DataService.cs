using System.Collections.Concurrent;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Groups;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FLRC.Leaderboards.Core.Data;

public class DataService : IDataService
{
	private readonly IDictionary<string, IResultsAPI> _resultsAPI;
	private readonly IGroupAPI _groupAPI;
	private readonly ICommunityAPI _communityAPI;
	private readonly ILogger _logger;
	private readonly TimeSpan _cacheLength;
	private readonly IDictionary<uint, Race> _races;
	private readonly uint _startListID;

	public DataService(IDictionary<string, IResultsAPI> resultsAPI, IGroupAPI groupAPI, ICommunityAPI communityAPI, IConfiguration configuration, ILoggerFactory loggerFactory)
	{
		_resultsAPI = resultsAPI;
		_groupAPI = groupAPI;
		_communityAPI = communityAPI;

		_logger = loggerFactory.CreateLogger("DataService");
		_cacheLength = TimeSpan.FromSeconds(configuration.GetValue<byte?>("APICacheLength") ?? 10);

		_startListID = configuration.GetValue<uint>("StartListRaceID");
		_races = configuration.GetSection("Races").GetChildren()
			.ToDictionary(c => c.GetValue<uint>("ID"), GetRace);
	}

	private static Race GetRace(IConfigurationSection section)
	{
		var race = section.Get<Race>();
		race.Courses = GetCourses(race, section);
		return race;
	}

	private static IReadOnlyCollection<Course> GetCourses(Race race, IConfiguration section)
		=> section.GetSection("Courses").GetChildren()
			.Select(c => new Course
			{
				Race = race,
				ID = uint.Parse(c.Value),
				Distance = new Distance(string.IsNullOrWhiteSpace(c.Key) || c.Key == Distance.DefaultKey ? section.GetValue<string>("Distance") : c.Key)
			})
			.OrderBy(c => c.Distance)
			.ToList();

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
		try
		{
			if (_athleteCacheTimestamp < DateTime.Now.Subtract(_cacheLength))
			{
				if (_startListID == 0)
				{
					await CacheAthletesFromResults();
				}
				else
				{
					await CacheAthletesFromStartList();
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

	private async Task CacheAthletesFromStartList()
	{
		var json = await _resultsAPI[nameof(WebScorer)].GetResults(_startListID);
		foreach (var athlete in json.GetProperty("Racers").EnumerateArray().Select(_resultsAPI[nameof(WebScorer)].Source.ParseAthlete))
		{
			CacheAthlete(athlete);
		}
	}

	private async Task CacheAthletesFromResults()
	{
		var results = await GetAllResults();
		var athletes = results.SelectMany(c => c.Results.Select(r => r.Athlete)).DistinctBy(a => a.ID);
		foreach (var athlete in athletes)
		{
			CacheAthlete(athlete);
		}
	}

	private void CacheAthlete(Athlete athlete)
	{
		if (!_athletes.ContainsKey(athlete.ID))
		{
			_athletes.Add(athlete.ID, athlete);
		}
	}

	public async Task<Course> GetResults(uint id, string distance = null)
	{
		var course = _races.SelectMany(r => r.Value.Courses).First(c => (c.ID == id || c.Race.ID == id) && (distance == null || c.Distance.Display == distance));
		if (course.ID == 0)
			return course;

		try
		{
			if (course.LastUpdated < DateTime.Now.Subtract(_cacheLength))
			{
				var getPosts = _communityAPI.GetPosts(course.Race.CommunityID);

				var results = await _resultsAPI[course.Race.Source].GetResults(course.ID);
				var resultsHash = results.ToString().GetHashCode();

				if (resultsHash != course.LastHash)
				{
					course.Results = _resultsAPI[course.Race.Source].Source.ParseCourse(course, results);
					course.LastHash = resultsHash;
				}

				var posts = await getPosts;
				course.Race.CommunityPosts = _communityAPI.ParsePosts(posts);
				var communityHash = posts.ToString().GetHashCode();

				if (resultsHash != course.LastHash || communityHash != course.Race.CommunityHash)
				{
					UpdateCommunityStars(course.Results, course.Race.CommunityPosts);
					course.Race.CommunityHash = communityHash;
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

	private static void UpdateCommunityStars(IEnumerable<Result> results, IReadOnlyCollection<Post> posts)
	{
		foreach (var result in results)
		{
			result.CommunityStars[StarType.GroupRun] = result.IsGroupRun() && !result.HasCommunityPointToday(StarType.GroupRun);
			result.CommunityStars[StarType.Story] = posts.Any(p => p.HasNarrative() && p.Matches(result)) && !result.HasCommunityPointToday(StarType.Story);
			result.CommunityStars[StarType.ShopLocal] = posts.Any(p => p.HasLocalBusiness() && p.Matches(result)) && !result.HasCommunityPointToday(StarType.ShopLocal);
		}
	}

	public async Task<IReadOnlyCollection<Course>> GetAllResults()
	{
		var tasks = _races.SelectMany(r => r.Value.Courses.Select(c => GetResults(c.ID, c.Distance.Display)));
		return await Task.WhenAll(tasks);
	}

	public async Task<IReadOnlyCollection<Athlete>> GetGroupMembers(string id)
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

	private IDictionary<string, Athlete[]> _groups;
	private DateTime _groupCacheTimestamp;

	private async Task<IDictionary<string, Athlete[]>> GetGroups()
	{
		if (_groupCacheTimestamp < DateTime.Now.Subtract(_cacheLength))
		{
			var athletes = await GetAthletes();
			var members = await _groupAPI.GetGroups();
			_groups = members.ToDictionary(m => m.Key,
				m => m.Value.Select(v => athletes.ContainsKey(v) ? athletes[v] : null).Where(a => a != null).ToArray());
			_groupCacheTimestamp = DateTime.Now;
		}

		return _groups;
	}
}