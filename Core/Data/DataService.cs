using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Races;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FLRC.Leaderboards.Core.Data;

public sealed class DataService : IDataService
{
	private readonly IDictionary<string, IResultsAPI> _resultsAPI;
	private readonly ICustomInfoAPI _customInfoAPI;
	private readonly ICommunityAPI _communityAPI;
	private readonly ILogger _logger;
	private readonly TimeSpan _cacheLength;
	private readonly IDictionary<uint, Race> _races;
	private readonly uint _startListID;

	public DataService(IDictionary<string, IResultsAPI> resultsAPI, ICustomInfoAPI customInfoAPI, ICommunityAPI communityAPI, IConfiguration configuration, ILoggerFactory loggerFactory)
	{
		_resultsAPI = resultsAPI;
		_customInfoAPI = customInfoAPI;
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

	private static Course[] GetCourses(Race race, IConfiguration section)
		=> section.GetSection("Courses").GetChildren()
			.Select(c => new Course
			{
				Race = race,
				ID = uint.Parse(c.Value ?? "0"),
				Distance = new Distance(string.IsNullOrWhiteSpace(c.Key) || c.Key == Distance.DefaultKey ? section.GetValue<string>("Distance") : c.Key)
			})
			.OrderBy(c => c.Distance)
			.ToArray();

	private readonly ConcurrentDictionary<uint, Athlete> _athletes = new();
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
		var aliases = await GetAliases();
		foreach (var athlete in json.GetProperty("Racers").EnumerateArray().Select(a => _resultsAPI[nameof(WebScorer)].Source.ParseAthlete(a, aliases)).Where(a => !_athletes.ContainsKey(a.ID)))
		{
			_athletes[athlete.ID] = athlete;
		}
	}

	private async Task CacheAthletesFromResults()
	{
		var results = await GetAllResults();
		var athletes = results.SelectMany(c => c.Results.Select(r => r.Athlete).Where(a => !_athletes.ContainsKey(a.ID))).DistinctBy(a => a.ID);
		foreach (var athlete in athletes)
		{
			_athletes[athlete.ID] = athlete;
		}
	}

	public async Task<Course> GetResults(uint id, string distance)
	{
		var course = _races.SelectMany(r => r.Value.Courses).First(c => (c.ID == id || c.Race.ID == id) && (distance == null || c.Distance.Display == distance));
		if (course.ID == 0)
			return course;

		if (course.LastUpdated < DateTime.Now.Subtract(_cacheLength))
		{
			await UpdateResults(course);
			await UpdateCommunityPosts(course.Race);
			SetCommunityStars(course);
			course.LastUpdated = DateTime.Now;
		}

		return course;
	}

	private async Task UpdateResults(Course course)
	{
		try
		{
			var results = await _resultsAPI[course.Race.Source].GetResults(course.ID);
			var resultsHash = results.ToString().GetHashCode();

			if (resultsHash != course.LastHash)
			{
				var aliases = GetAliases();
				course.Results = _resultsAPI[course.Race.Source].Source.ParseCourse(course, results, await aliases);
				course.LastHash = resultsHash;
			}
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Could not retrieve results");
		}
	}

	private DateTime _cachedAliasTimestamp;
	private IDictionary<string, string> _cachedAliases = new ConcurrentDictionary<string, string>();

	private async Task<IDictionary<string, string>> GetAliases()
	{
		if (_cachedAliasTimestamp < DateTime.Now.Subtract(_cacheLength))
		{
			_cachedAliases = await _customInfoAPI.GetAliases();
			_cachedAliasTimestamp = DateTime.Now;
		}

		return _cachedAliases;
	}

	private async Task UpdateCommunityPosts(Race race)
	{
		try
		{
			if (race.CommunityID == 0)
			{
				return;
			}

			var posts = await _communityAPI.GetPosts(race.CommunityID);
			var communityHash = string.Join(string.Empty, posts.Select(p => p.ToString().GetHashCode())).GetHashCode();

			if (communityHash != race.CommunityHash)
			{
				race.CommunityPosts = _communityAPI.ParsePosts(posts);
				race.CommunityHash = communityHash;
			}
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Could not retrieve community posts");
		}
	}

	private static void SetCommunityStars(Course course)
	{
		foreach (var result in course.Results)
		{
			result.CommunityStars[StarType.GroupRun] = result.IsGroupRun() && !result.HasCommunityStarToday(StarType.GroupRun);
			result.CommunityStars[StarType.Story] = result.Course.Race.CommunityPosts.Any(p => p.Matches(result) && p.HasNarrative()) && !result.HasCommunityStarToday(StarType.Story);
		}
	}

	public async Task<Course[]> GetAllResults()
	{
		var tasks = _races.SelectMany(r => r.Value.Courses.Select(c => GetResults(c.ID, c.Distance.Display)));
		return await Task.WhenAll(tasks);
	}

	public async Task<Athlete[]> GetGroupMembers(string id)
	{
		try
		{
			var groups = await GetGroups();
			return groups[id];
		}
		catch (Exception e)
		{
			if (e is not HttpRequestException { StatusCode: HttpStatusCode.NotFound })
			{
				_logger.LogWarning(e, "Could not retrieve group members for {id}", id);
			}

			return [];
		}
	}

	private IDictionary<string, Athlete[]> _groups = new ConcurrentDictionary<string, Athlete[]>();
	private DateTime _groupCacheTimestamp;

	private async Task<IDictionary<string, Athlete[]>> GetGroups()
	{
		if (_groupCacheTimestamp < DateTime.Now.Subtract(_cacheLength))
		{
			var athletes = await GetAthletes();
			var members = await _customInfoAPI.GetGroups();
			_groups = members.ToDictionary(m => m.Key,
				m => m.Value.Select(v => athletes.TryGetValue(v, out var athlete) ? athlete : null).Where(a => a is not null).ToArray());
			_groupCacheTimestamp = DateTime.Now;
		}

		return _groups;
	}

	private IDictionary<Athlete, DateOnly> _personal = new ConcurrentDictionary<Athlete, DateOnly>();
	private DateTime _personalCacheTimestamp;

	public async Task<IDictionary<Athlete, DateOnly>> GetPersonalCompletions()
	{
		if (_personalCacheTimestamp < DateTime.Now.Subtract(_cacheLength))
		{
			await UpdatePersonalCompletions();
		}

		return _personal;
	}

	private async Task UpdatePersonalCompletions()
	{
		try
		{
			var athletes = await GetAthletes();
			var completions = await _customInfoAPI.GetPersonalCompletions();
			_personal = completions.ToDictionary(c => athletes[c.Key], c => c.Value);
			_personalCacheTimestamp = DateTime.Now;
		}
		catch (Exception e)
		{
			if (e is not HttpRequestException { StatusCode: HttpStatusCode.NotFound })
			{
				_logger.LogWarning(e, "Could not retrieve personal completions");
			}
		}
	}

	public async Task<User[]> GetCommunityUsers()
	{
		var users = await _communityAPI.GetUsers();
		return users.Select(ParseUser).ToArray();
	}

	public async Task<User[]> GetCommunityGroupMembers(string groupID)
	{
		var users = await _communityAPI.GetMembers(groupID);
		return users.Select(ParseUser).ToArray();
	}

	public async Task AddCommunityGroupMembers(IDictionary<string, string[]> groupAdditions)
	{
		foreach (var group in groupAdditions)
		{
			var info = await _communityAPI.GetGroup(group.Key);
			var id = info.GetProperty("id").GetUInt16();
			await _communityAPI.AddMembers(id, group.Value);
		}
	}

	private static User ParseUser(JsonElement json)
		=> new()
		{
			ID = json.GetProperty("id").GetUInt16(),
			Name = json.GetProperty("name").GetString(),
			Username = json.GetProperty("username").GetString()
		};
}