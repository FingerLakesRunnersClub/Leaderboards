using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChallengeDashboard
{
    public class CourseService
    {
        private readonly API _api;

        private readonly IDictionary<uint, Course> _courseCache = new Dictionary<uint, Course>();

        public CourseService(API api)
        {
            _api = api;
        }

        public async Task<Course> GetCourse(uint id)
        {
            if (!_courseCache.ContainsKey(id))
            {
                var course = await _api.GetCourse(id);
                _courseCache.Add(id, course);
            }

            return _courseCache[id];
        }

    }
}
