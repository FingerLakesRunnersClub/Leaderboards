using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FLRC.ChallengeDashboard
{
    public class DataService : IDataService
    {
        private readonly IDataAPI _api;
        private readonly ILogger _logger;
        private readonly TimeSpan _cacheLength;
        private readonly IDictionary<uint, Course> _courses;
        private readonly uint _startListID;

        public DataService(IDataAPI api, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _api = api;
            _logger = loggerFactory.CreateLogger("DataService");
            _cacheLength = TimeSpan.FromSeconds(configuration.GetValue<byte?>("APICacheLength") ?? 10);

            _startListID = configuration.GetValue<uint>("StartListRaceID");
            _courses = configuration.GetSection("Courses").GetChildren()
                .ToDictionary(c => uint.Parse(c["ID"]), c => c.Get<Course>());
            foreach (var course in _courses.Values)
                course.Meters = DataParser.ParseDistance(course.Distance);
        }

        public IDictionary<uint, string> CourseNames
            => _courses.ToDictionary(c => c.Key, c => c.Value.Name);

        private IDictionary<uint, Athlete> _athletes = new Dictionary<uint, Athlete>();
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
                    var json = await _api.GetResults(_startListID);
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
            try
            {
                if (_courses[id].LastUpdated < DateTime.Now.Subtract(_cacheLength))
                {
                    var json = await _api.GetResults(id);
                    var newHash = json.ToString()?.GetHashCode() ?? 0;
                    if (newHash != _courses[id].LastHash)
                    {
                        _courses[id].Results = DataParser.ParseCourse(json);
                        _courses[id].LastHash = newHash;
                    }

                    _courses[id].LastUpdated = DateTime.Now;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Could not retrieve results");
            }

            return _courses[id];
        }

        public async Task<IEnumerable<Course>> GetAllResults()
        {
            var tasks = _courses.Select(c => GetResults(c.Key)).ToArray();
            await Task.WhenAll(tasks);
            return tasks.Select(t => t.GetAwaiter().GetResult());
        }
    }
}