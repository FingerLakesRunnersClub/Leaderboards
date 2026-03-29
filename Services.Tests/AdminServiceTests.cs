using FLRC.Leaderboards.Model;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public sealed class AdminServiceTests
{
	[Fact]
	public async Task CanAddAdmin()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AdminService(db);

		var admin = new Admin { ID = Guid.NewGuid() };

		//act
		await service.Add(admin);

		//assert
		Assert.Equal(1, db.Set<Admin>().Count());
	}

	[Fact]
	public async Task CanRemoveAdmin()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AdminService(db);

		var id = Guid.NewGuid();
		var admin = new Admin { ID = id };
		await db.AddAsync(admin);
		await db.SaveChangesAsync();

		//act
		await service.Delete(admin);

		//assert
		Assert.Empty(db.Set<Admin>());
	}
}