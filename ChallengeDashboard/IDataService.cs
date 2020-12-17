using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLRC.ChallengeDashboard
{
    public interface IDataService
    {
        IDictionary<uint, string> CourseNames { get; }

        Task<Course> GetResults(uint id);
        Task<IEnumerable<Course>> GetAllResults();
    }
}