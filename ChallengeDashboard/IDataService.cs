using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLRC.ChallengeDashboard
{
    public interface IDataService
    {
        Task<Course> GetResults(uint id);
        Task<IEnumerable<Course>> GetAllResults();
    }
}