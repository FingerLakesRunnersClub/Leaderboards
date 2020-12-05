using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChallengeDashboard
{
    public class DataService
    {
        private readonly API _api;

        private readonly IDictionary<uint, Course> _courseCache = new Dictionary<uint, Course>();

        public DataService(API api) => _api = api;

        public async Task<Course> GetCourse(uint id)
        {
            if (_courseCache.ContainsKey(id)) return _courseCache[id];
            
            var course = await _api.GetCourse(id);
            _courseCache.Add(id, course);
            return _courseCache[id];
        }

        public async Task<IEnumerable<Course>> GetAllCourses(IEnumerable<uint> ids)
        {
            var tasks = ids.Select(GetCourse).ToArray();
            await Task.WhenAll(tasks);
            return tasks.Select(t => t.GetAwaiter().GetResult());
        }
    }
}
