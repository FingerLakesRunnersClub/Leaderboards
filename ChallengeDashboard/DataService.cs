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
        private readonly IEnumerable<uint> _courseIDs;

        private readonly IDictionary<uint, Course> _courseCache = new Dictionary<uint, Course>();

        public DataService(IDataAPI api, IConfiguration configuration)
        {
            _api = api;
            _courseIDs = configuration.GetSection("Courses").AsEnumerable()
                .Where(id => !string.IsNullOrWhiteSpace(id.Value))
                .Select(id => Convert.ToUInt32(id.Value));
        }

        public async Task<Course> GetCourse(uint id)
        {
            if (!_courseCache.ContainsKey(id))
            {
                var json = await _api.GetCourse(id);
                var course = DataParser.ParseCourse(json);
                _courseCache.Add(id, course);
            }

            return _courseCache[id];
        }

        public async Task<IEnumerable<Course>> GetAllCourses()
        {
            var tasks = _courseIDs.Select(GetCourse).ToArray();
            await Task.WhenAll(tasks);
            return tasks.Select(t => t.GetAwaiter().GetResult());
        }
    }
}
