using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface IDataService<T> where T : Identifiable<Guid>
{
	Task<T[]> All();
	Task<T> Get(Guid id);
	Task Add(T obj);
	Task Update(T obj, T updated);
	Task Delete(T obj);
}