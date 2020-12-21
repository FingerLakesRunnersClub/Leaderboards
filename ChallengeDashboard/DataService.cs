using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FLRC.ChallengeDashboard
{
    public class DataService : IDataService
    {
        private readonly IDataAPI _api;
        private readonly IDictionary<uint, Course> _courses;

        public DataService(IDataAPI api, IConfiguration configuration)
        {
            _api = api;
            _courses = configuration.GetSection("Courses").GetChildren()
                .ToDictionary(c => uint.Parse(c["ID"]), c => c.Get<Course>());

            foreach (var course in _courses.Values)
                course.Meters = DataParser.ParseDistance(course.Distance);
        }

        public IDictionary<uint, string> CourseNames
            => _courses.ToDictionary(c => c.Key, c => c.Value.Name);

        public async Task<Course> GetResults(uint id)
        {
            if (_courses[id].LastUpdated < DateTime.Now.Subtract(TimeSpan.FromSeconds(5)))
            {
                var json = await _api.GetResults(id);
                var newHash = json.GetHashCode();
                if (newHash != _courses[id].LastHash)
                {
                    _courses[id].Results = DataParser.ParseCourse(json);
                    _courses[id].LastHash = newHash;
                }
                _courses[id].LastUpdated = DateTime.Now;
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
