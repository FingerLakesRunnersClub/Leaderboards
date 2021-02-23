using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLRC.ChallengeDashboard
{
    public interface IDataService
    {
        IDictionary<uint, string> CourseNames { get; }

        Task<Athlete> GetAthlete(uint id);
        Task<IDictionary<uint, Athlete>> GetAthletes();
        
        Task<Course> GetResults(uint id);
        Task<IEnumerable<Course>> GetAllResults();
    }
}