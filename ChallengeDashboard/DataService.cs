﻿using System;
using System.Collections.Concurrent;
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

            CourseNames = _courses.ToDictionary(c => c.Key, c => c.Value.Name);
            CourseDistances = _courses.ToDictionary(c => c.Key, c => c.Value.Miles);
            Links = configuration.GetSection("Links").GetChildren().ToDictionary(c => c.Key, c => c.Value);
        }

        public IDictionary<uint, string> CourseNames { get; }
        public IDictionary<uint, double> CourseDistances { get; }
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
            var course = _courses[id];
            
            try
            {
                if (course.LastUpdated < DateTime.Now.Subtract(_cacheLength))
                {
                    var json = await _api.GetResults(id);
                    var newHash = json.ToString()?.GetHashCode() ?? 0;
                    if (newHash != course.LastHash)
                    {
                        course.Results = DataParser.ParseCourse(json);
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
    }
}