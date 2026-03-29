using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public sealed class AdminService(DB db) : IAdminService
{
	public async Task<Admin[]> All()
		=> throw new NotImplementedException();

	public async Task<Admin> Get(Guid id)
		=> throw new NotImplementedException();

	public async Task<bool> Verify(Guid id)
		=> await db.Set<Admin>().FindAsync(id) is not null;

	public async Task Add(Admin admin)
	{
		await db.AddAsync(admin);
		await db.SaveChangesAsync();
	}

	public async Task Update(Admin obj, Admin updated)
		=> throw new NotImplementedException();

	public async Task Delete(Admin admin)
	{
		db.Remove(admin);
		await db.SaveChangesAsync();
	}
}