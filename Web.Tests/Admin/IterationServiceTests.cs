using FLRC.Leaderboards.Data.Models;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class IterationServiceTests
{
	[Fact]
	public async Task CanGetAllIterations()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new IterationService(db);

		var series = new Series { ID = Guid.NewGuid(), Key = "Test", Name = "Test" };
		var i1 = new Iteration { ID = Guid.NewGuid(), Name = "Test 1", Series = series };
		var i2 = new Iteration { ID = Guid.NewGuid(), Name = "Test 2", Series = series };
		await db.AddRangeAsync(i1, i2);
		await db.SaveChangesAsync();

		//act
		var iterations = await service.GetAllIterations();

		//assert
		Assert.Equal(2, iterations.Length);
	}

	[Fact]
	public async Task CanGetIteration()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new IterationService(db);

		var series = new Series { ID = Guid.NewGuid(), Key = "Test", Name = "Test" };
		var i1 = new Iteration { ID = Guid.NewGuid(), Name = "Test 1", Series = series };
		var i2 = new Iteration { ID = Guid.NewGuid(), Name = "Test 2", Series = series };
		await db.AddRangeAsync(i1, i2);
		await db.SaveChangesAsync();

		//act
		var iteration = await service.GetIteration(i1.ID);

		//assert
		Assert.Equal("Test 1", iteration.Name);
	}

	[Fact]
	public async Task CanAddIteration()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new IterationService(db);

		var series = new Series { ID = Guid.NewGuid(), Key = "Test", Name = "Test" };
		await db.AddAsync(series);
		await db.SaveChangesAsync();

		//act
		var iteration = new Iteration { Name = "Test 1", Series = series };
		await service.AddIteration(series.ID, iteration, []);

		//assert
		Assert.Equal("Test 1", db.Set<Iteration>().Single().Name);
	}

	[Fact]
	public async Task CanUpdateIteration()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new IterationService(db);

		var series = new Series { ID = Guid.NewGuid(), Key = "Test", Name = "Test" };
		var iteration = new Iteration { ID = Guid.NewGuid(), Name = "Test 1", Series = series };
		await db.AddAsync(iteration);
		await db.SaveChangesAsync();

		//act
		var updated = new Iteration { Name = "Test 2" };
		await service.UpdateIteration(iteration, updated, []);

		//assert
		Assert.Equal("Test 2", db.Set<Iteration>().Single().Name);
	}

	[Fact]
	public async Task CanUpdateIterationRaces()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new IterationService(db);

		var series = new Series { ID = Guid.NewGuid(), Key = "Test", Name = "Test" };
		var r1 = new Race { ID = Guid.NewGuid(), Name = "Race 1", Type = "Trail" };
		var r2 = new Race { ID = Guid.NewGuid(), Name = "Race 2", Type = "Trail" };
		var r3 = new Race { ID = Guid.NewGuid(), Name = "Race 3", Type = "Trail" };
		var iteration = new Iteration { ID = Guid.NewGuid(), Name = "Test 1", Series = series, Races = [r1, r2]};
		await db.AddRangeAsync(iteration, r1, r2, r3);
		await db.SaveChangesAsync();

		//act
		var updated = new Iteration { Name = "Test 2" };
		await service.UpdateIteration(iteration, updated, [r1.ID, r3.ID]);

		//assert
		var races = db.Set<Iteration>().Single().Races.OrderBy(r => r.Name).ToArray();
		Assert.Equal(2, races.Length);
		Assert.Equal("Race 1", races[0].Name);
		Assert.Equal("Race 3", races[1].Name);
	}
}