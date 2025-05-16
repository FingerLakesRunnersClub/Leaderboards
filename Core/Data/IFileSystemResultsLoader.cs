using FLRC.Leaderboards.Core.Races;

namespace FLRC.Leaderboards.Core.Data;

public interface IFileSystemResultsLoader
{
	Race[] GetRaces();
	Task<IDictionary<uint, Course>> GetAllResults();
}