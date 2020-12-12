using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLRC.ChallengeDashboard
{
    public interface IDataService
    {
        Task<Course> GetCourse(uint id);
        Task<IEnumerable<Course>> GetAllCourses(IEnumerable<uint> ids);
    }
}