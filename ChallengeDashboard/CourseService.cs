using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace ChallengeDashboard
{
    public class CourseService
    {
        private readonly API _api;

        private readonly IDictionary<uint, Course> _courseCache = new Dictionary<uint, Course>();

        public CourseService(API api) => _api = api;

        public async Task<Course> GetCourse(uint id)
        {
            if (_courseCache.ContainsKey(id)) return _courseCache[id];
            
            var course = await _api.GetCourse(id);
            _courseCache.Add(id, course);
            return _courseCache[id];
        }

        public async Task<IEnumerable<Course>> All(IEnumerable<uint> ids)
        {
            var tasks = ids.Select(GetCourse).ToArray();
            await Task.WhenAll(tasks);
            return tasks.Select(t => t.GetAwaiter().GetResult());
        }
    }
}
