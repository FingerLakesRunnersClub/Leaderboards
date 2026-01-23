using FLRC.Leaderboards.Data.Models;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public class SeriesServiceTests
{
	[Fact]
	public async Task CanGetAllSeries()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new SeriesService(db);

		var s1 = new Series { ID = Guid.NewGuid(), Key = "Test1", Name = "Test 1" };
		var s2 = new Series { ID = Guid.NewGuid(), Key = "Test2", Name = "Test 2" };
		await db.AddRangeAsync(s1, s2);
		await db.SaveChangesAsync();

		//act
		var series = await service.GetAllSeries();

		//assert
		Assert.Equal(2, series.Length);
	}

	[Fact]
	public async Task CanGetSeries()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new SeriesService(db);

		var s1 = new Series { ID = Guid.NewGuid(), Key = "Test1", Name = "Test 1" };
		var s2 = new Series { ID = Guid.NewGuid(), Key = "Test2", Name = "Test 2" };
		await db.AddRangeAsync(s1, s2);
		await db.SaveChangesAsync();

		//act
		var series = await service.GetSeries(s1.ID);

		//assert
		Assert.Equal("Test 1", series.Name);
	}

	[Fact]
	public async Task CanAddSeries()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new SeriesService(db);

		//act
		var series = new Series { ID = Guid.NewGuid(), Key = "Test", Name = "Test" };
		await service.AddSeries(series, new Dictionary<string, bool>(), new Dictionary<string, string>());

		//assert
		Assert.Equal("Test", db.Set<Series>().Single().Name);
	}

	[Fact]
	public async Task CanUpdateSeries()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new SeriesService(db);

		var series = new Series { ID = Guid.NewGuid(), Key = "Test", Name = "Test" };
		await db.AddAsync(series);
		await db.SaveChangesAsync();

		//act
		var updated = new Series { Key = "Test", Name = "Test 2" };
		await service.UpdateSeries(series, updated, new Dictionary<string, bool>(), new Dictionary<string, string>());

		//assert
		Assert.Equal("Test 2", db.Set<Series>().Single().Name);
	}
}