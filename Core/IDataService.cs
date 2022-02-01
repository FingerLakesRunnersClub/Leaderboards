using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLRC.Leaderboards.Core;

public interface IDataService
{
	IDictionary<uint, string> CourseNames { get; }
	IDictionary<string, string> Links { get; }

	Task<Athlete> GetAthlete(uint id);
	Task<IDictionary<uint, Athlete>> GetAthletes();

	Task<Course> GetResults(uint id);
	Task<IEnumerable<Course>> GetAllResults();

	Task<IEnumerable<Athlete>> GetGroupMembers(string id);
}