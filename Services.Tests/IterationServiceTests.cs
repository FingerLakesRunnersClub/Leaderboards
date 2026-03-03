using FLRC.Leaderboards.Model;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

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
	public async Task CanFindCurrentIteration()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new IterationService(db);

		var series = new Series { ID = Guid.NewGuid(), Key = "Test", Name = "Test" };
		var past = new Iteration { ID = Guid.NewGuid(), Name = "Past", StartDate = new DateOnly(DateTime.Today.Year - 1, 1, 1), EndDate = new DateOnly(DateTime.Today.Year - 1, 12, 31), Series = series };
		var present = new Iteration { ID = Guid.NewGuid(), Name = "Present", StartDate = new DateOnly(DateTime.Today.Year, 1, 1), EndDate = new DateOnly(DateTime.Today.Year, 12, 31), Series = series };
		var future = new Iteration { ID = Guid.NewGuid(), Name = "Future", StartDate = new DateOnly(DateTime.Today.Year + 1, 1, 1), EndDate = new DateOnly(DateTime.Today.Year +- 1, 12, 31), Series = series };
		await db.AddRangeAsync(past, present, future);
		await db.SaveChangesAsync();

		//act
		var iteration = await service.FindCurrentIteration();

		//assert
		Assert.Equal("Present", iteration!.Name);
	}

	[Fact]
	public async Task CanFindMostRecentIteration()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new IterationService(db);

		var series = new Series { ID = Guid.NewGuid(), Key = "Test", Name = "Test" };
		var old = new Iteration { ID = Guid.NewGuid(), Name = "Old", StartDate = new DateOnly(DateTime.Today.Year - 2, 1, 1), EndDate = new DateOnly(DateTime.Today.Year - 2, 12, 31), Series = series };
		var recent = new Iteration { ID = Guid.NewGuid(), Name = "Recent", StartDate = new DateOnly(DateTime.Today.Year - 1, 1, 1), EndDate = new DateOnly(DateTime.Today.Year - 1, 12, 31), Series = series };
		var future = new Iteration { ID = Guid.NewGuid(), Name = "Future", StartDate = new DateOnly(DateTime.Today.Year + 1, 1, 1), EndDate = new DateOnly(DateTime.Today.Year +- 1, 12, 31), Series = series };
		await db.AddRangeAsync(old, recent, future);
		await db.SaveChangesAsync();

		//act
		var iteration = await service.FindMostRecentIteration();

		//assert
		Assert.Equal("Recent", iteration!.Name);
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
		await service.AddIteration(series.ID, iteration);

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
		await service.UpdateIteration(iteration, updated);

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
		var r1 = new Race { ID = Guid.NewGuid(), Name = "Race 1", Type = "Trail", Description = "test" };
		var r2 = new Race { ID = Guid.NewGuid(), Name = "Race 2", Type = "Trail", Description = "test" };
		var r3 = new Race { ID = Guid.NewGuid(), Name = "Race 3", Type = "Trail", Description = "test" };
		var iteration = new Iteration { ID = Guid.NewGuid(), Name = "Test 1", Series = series, Races = [r1, r2]};
		await db.AddRangeAsync(iteration, r1, r2, r3);
		await db.SaveChangesAsync();

		//act
		await service.UpdateRaces(iteration, [r1, r3]);

		//assert
		var races = db.Set<Iteration>().Single().Races.OrderBy(r => r.Name).ToArray();
		Assert.Equal(2, races.Length);
		Assert.Equal("Race 1", races[0].Name);
		Assert.Equal("Race 3", races[1].Name);
	}

	[Fact]
	public async Task CanUpdateIterationRegistrations()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new IterationService(db);

		var series = new Series { ID = Guid.NewGuid(), Key = "Test", Name = "Test" };
		var a1 = new Athlete { ID = Guid.NewGuid(), Name = "A1" };
		var a2 = new Athlete { ID = Guid.NewGuid(), Name = "A2" };
		var a3 = new Athlete { ID = Guid.NewGuid(), Name = "A3" };
		var iteration = new Iteration { ID = Guid.NewGuid(), Name = "Test 1", Series = series, Athletes = [a1, a2]};
		await db.AddRangeAsync(iteration, a1, a2, a3);
		await db.SaveChangesAsync();

		//act
		await service.UpdateRegistrations(iteration, [a1, a3]);

		//assert
		var athletes = db.Set<Iteration>().Single().Athletes.OrderBy(r => r.Name).ToArray();
		Assert.Equal(2, athletes.Length);
		Assert.Equal("A1", athletes[0].Name);
		Assert.Equal("A3", athletes[1].Name);
	}
}