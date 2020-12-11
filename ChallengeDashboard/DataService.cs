using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChallengeDashboard
{
    public class DataService
    {
        private readonly IDataAPI _api;

        private readonly IDictionary<uint, Course> _courseCache = new Dictionary<uint, Course>();

        public DataService(IDataAPI api) => _api = api;

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

        public async Task<IEnumerable<Course>> GetAllCourses(IEnumerable<uint> ids)
        {
            var tasks = ids.Select(GetCourse).ToArray();
            await Task.WhenAll(tasks);
            return tasks.Select(t => t.GetAwaiter().GetResult());
        }
    }
}
